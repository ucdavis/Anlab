using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Data;
using AnlabMvc.Models.Configuration;
using AnlabMvc.Services;
using AspNetCore.Security.CAS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
           
            services.Configure<AzureOptions>(Configuration.GetSection("Authentication:Azure"));

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IDirectorySearchService, DirectorySearchService>();
            services.AddTransient<IDbInitializationService, DbInitializationService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
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

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            var casOptions = new CasOptions
            {
                CasServerUrlBase = "https://cas.ucdavis.edu/cas/",
                Events = new CasEvents
                {
                    OnCreatingTicket = async ctx =>
                    {
                        var identity = ctx.Principal.Identity as ClaimsIdentity;

                        if (identity == null)
                        {
                            return;
                        }

                        var kerb = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                        // look up user info and add as claims
                        var user = await app.ApplicationServices.GetService<IDirectorySearchService>().GetByKerb(kerb);

                        if (user != null)
                        {                            
                            identity.AddClaim(new Claim(ClaimTypes.Email, user.Mail));
                            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.GivenName));
                            identity.AddClaim(new Claim(ClaimTypes.Surname, user.Surname));

                            // Cas already adds a name param but it's a duplicate of nameIdentifier, so let's replace with something useful
                            identity.RemoveClaim(identity.FindFirst(ClaimTypes.Name));
                            identity.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));

                        }
                    }
                }
            };
            app.UseCasAuthentication(casOptions);

            app.UseGoogleAuthentication(new GoogleOptions()
            {
                ClientId = Configuration["Authentication:Google:ClientId"],
                ClientSecret = Configuration["Authentication:Google:ClientSecret"]
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
