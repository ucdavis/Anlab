using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Core.Services;
using AnlabMvc.Models.Configuration;
using AnlabMvc.Services;
using AspNetCore.Security.CAS;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using StackifyLib;

namespace AnlabMvc
{
    public class Startup
    {
        private IDirectorySearchService _directorySearchService;
        private IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();            

            StackifyLib.Config.Environment = env.EnvironmentName;
            _environment = env;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.Configure<AzureOptions>(Configuration.GetSection("Authentication:Azure"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<ConnectionSettings>(Configuration.GetSection("ConnectionSettings"));
            services.Configure<CyberSourceSettings>(Configuration.GetSection("CyberSourceSettings"));
            services.Configure<FinancialSettings>(Configuration.GetSection("Financial"));
            services.Configure<EmailSettings>(Configuration.GetSection("Email"));

            // Add framework services.
            if (_environment.IsDevelopment())
            {
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
                .AddCAS("UCDavis", options => {
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
#if !DEBUG
                options.Filters.Add(new RequireHttpsAttribute());
#endif
            });


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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.ConfigureStackifyLogging(Configuration);
#if !DEBUG
            var options = new RewriteOptions()
                .AddRedirectToHttps();
            app.UseRewriter(options);
#endif

            _directorySearchService = app.ApplicationServices.GetService<IDirectorySearchService>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                // TODO: if we want to use auto-refresh browerlink. Might conflict with webpack
                //app.UseBrowserLink();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Error/Error");
            }

            app.UseStatusCodePagesWithRedirects("/Error/Index/{0}");
            app.UseStatusCodePagesWithReExecute("/Error/Index/{0}");

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "pages",
                    template: "pages/{id}",
                    defaults: new { controller = "Pages", action = "ViewPage" });

                //No fallback
            });
        }
    }
}
