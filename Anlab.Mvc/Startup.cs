using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlite("Data Source=anlab.db")
              // options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/LogIn");

            services.AddAuthentication()
                .AddOpenIdConnect("UCDavis", options =>
                {
                    options.Events.OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.SetParameter("domain_hint", "ucdavis.edu");

                        return Task.FromResult(0);
                    };
                    options.Events.OnTokenValidated = async context =>
                    {
                        var identity = context.Principal.Identity as ClaimsIdentity;

                        if (identity == null)
                        {
                            return;
                        }

                        // email comes across in both name claim and upn
                        var email = identity.FindFirst(ClaimTypes.Upn).Value;

                        // look up user info and add as claims
                        var user = await _directorySearchService.GetByEmail(email);

                        if (user != null)
                        {
                            // Should we bother replacing via directory service?
                            identity.AddClaim(new Claim(ClaimTypes.Email, user.Mail));
                            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.GivenName));
                            identity.AddClaim(new Claim(ClaimTypes.Surname, user.Surname));

                            // Cas already adds a name param but it's a duplicate of nameIdentifier, so let's replace with something useful
                            identity.RemoveClaim(identity.FindFirst(ClaimTypes.Name));
                            identity.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));

                            //replace with kerberos
                            identity.RemoveClaim(identity.FindFirst(ClaimTypes.NameIdentifier));
                            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Kerberos));
                        }
                    };
                    options.ClientId = "c631afcb-0795-4546-844d-9fe7759ae620";
                    options.Authority = "https://login.microsoftonline.com/ucdavis365.onmicrosoft.com";
                    options.SignedOutRedirectUri = "https://localhost:44349/";
                })
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });

            // TODO: require HTTPS in production.  In development it is only needed for federated auth
            services.AddMvc(options =>
            {
                // options.Filters.Add(new RequireHttpsAttribute());
            });

            // app services
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<IDirectorySearchService, DirectorySearchService>();
            services.AddTransient<IDbInitializationService, DbInitializationService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ViewRenderService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IOrderMessageService, OrderMessageService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddTransient<ILabworksService, FakeLabworksService>(); //TODO: Replace with non fake one.
            services.AddTransient<ILabworksService, LabworksService>();
            services.AddTransient<IFileStorageService, FileStorageService>();
            services.AddSingleton<IDataSigningService, DataSigningService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _directorySearchService = app.ApplicationServices.GetService<IDirectorySearchService>();
            app.ConfigureStackifyLogging(Configuration);

            Log.Logger = new LoggerConfiguration().WriteTo.Stackify().CreateLogger();

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
                app.UseExceptionHandler("/Home/Error");
            }

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

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}

