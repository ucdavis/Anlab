using System;
using System.IO;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Core.Models.AggieEnterpriseModels;
using Anlab.Core.Services;
using AnlabMvc.Middleware;
using AnlabMvc.Models.Configuration;
using AnlabMvc.Services;
using AspNetCore.Security.CAS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MvcReact;
using Serilog;

namespace AnlabMvc
{
    public class Startup
    {
        private IDirectorySearchService _directorySearchService;
        private IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _environment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.Configure<AzureOptions>(Configuration.GetSection("Authentication:Azure"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<AggieEnterpriseSettings>(Configuration.GetSection("AggieEnterprise"));
            services.Configure<ConnectionSettings>(Configuration.GetSection("ConnectionSettings"));
            services.Configure<CyberSourceSettings>(Configuration.GetSection("CyberSourceSettings"));
            services.Configure<FinancialSettings>(Configuration.GetSection("Financial"));
            services.Configure<EmailSettings>(Configuration.GetSection("Email"));
            services.Configure<ESignatureOptions>(Configuration.GetSection("ESigning"));

            // Add framework services.
            if (_environment.IsDevelopment())
            {
                services.AddDatabaseDeveloperPageExceptionFilter();

                if (Configuration.GetSection("Dev:UseSql").Value == "Yes")
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                    );
                }
                else
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite("Data Source=anlab.db")
                    );
                }
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                );
            }

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/LogIn");

            services.AddAuthentication()
                .AddCAS("UCDavis", options =>
                {
                    options.CasServerUrlBase = Configuration["AppSettings:CasBaseUrl"];
                    // options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });

            // TODO: require HTTPS in production.  In development it is only needed for federated auth
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add<SerilogControllerActionFilter>();
            });

            // Init services for hybrid mvc/react app
            services.AddMvcReact();

            // app services
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<IDirectorySearchService, IetWsSearchService>();
            services.AddTransient<IDbInitializationService, DbInitializationService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ViewRenderService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<ISlothService, SlothService>();
            services.AddTransient<IOrderMessageService, OrderMessageService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            if (Configuration.GetSection("Dev:UseFakeLabworks").Value == "Yes")
            {
                services.AddTransient<ILabworksService, FakeLabworksService>(); //Fake one for testing only
            }
            else
            {
                services.AddTransient<ILabworksService, LabworksService>();
            }
            services.AddTransient<IFileStorageService, FileStorageService>();
            services.AddSingleton<IDataSigningService, DataSigningService>();
            services.AddTransient<IFinancialService, FinancialService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IAggieEnterpriseService, AggieEnterpriseService>();
            services.AddTransient<IDocumentSigningService, DocumentSigningService>();

            // Used by dynamic scripts/styles loader
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory())); // lgtm [cs/local-not-disposed]

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<MvcReactOptions> mvcReactOptions)
        {
            _directorySearchService = app.ApplicationServices.GetService<IDirectorySearchService>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // TODO: if we want to use auto-refresh browerlink. Might conflict with webpack
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error/Error");
            }

            app.UseStatusCodePagesWithRedirects("/Error/Index/{0}");
            app.UseStatusCodePagesWithReExecute("/Error/Index/{0}");

            app.UseStaticFiles();
            app.UseMvcReactStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<LogUserNameMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}",
                    constraints: new { controller = mvcReactOptions.Value.ExcludeHmrPathsRegex });

                endpoints.MapControllerRoute(
                    name: "pages",
                    pattern: "pages/{id}",
                    defaults: new { controller = "Pages", action = "ViewPage" });
            });

            // During development, SPA will kick in for all remaining paths
            app.UseMvcReact();

        }
    }
}
