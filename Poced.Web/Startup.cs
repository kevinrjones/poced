using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poced.Identity.Shared;
using Poced.Repository.Contexts;

namespace poced.web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PocedDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default")));

            services.AddIdentity<PocedUser, IdentityRole>()
                .AddEntityFrameworkStores<PocedDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            /*
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {                
                AuthenticationType = "Cookie",
                LoginPath = new PathString("/login")
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ExternalCookie",
                AuthenticationMode = AuthenticationMode.Passive,
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
            });

            */
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                })
                .AddGoogle(options =>
                {
                    //googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    //googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    options.ClientId = "656376280253-dj3e3qqhmbcp4oir5gfk8btm17v8b9om.apps.googleusercontent.com";
                    options.ClientSecret = "HvijClsL9XzNeVICelRuWc5W";
                });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /*
                 private void RegisterTypes(ContainerBuilder builder)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var perfLogger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            var usageLogger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.File($@"{baseDirectory}\logs\usage.log")
                .CreateLogger();

            var errorLogger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            var diagnosticLogger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            var connectionString = ConfigurationManager.ConnectionStrings["pocedEntities"].ConnectionString;
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var repositoryAssemblies = Assembly.Load("Poced.Services");
            builder.RegisterAssemblyTypes(repositoryAssemblies).AsImplementedInterfaces();

            builder.RegisterType<ArticlesRepository>().As<IArticlesRepository>().WithParameter(new NamedParameter("connectionString", connectionString));
            builder.RegisterType<UsersRepository>().As<IUsersRepository>().WithParameter(new NamedParameter("connectionString", connectionString));
            builder.RegisterType<AppConfiguration>().As<IConfiguration>();


            // todo: autofac named parameters with registered values
            builder.RegisterType<PocedSerlogLogger>().As<IPocedLogger>().WithParameter(new NamedParameter("perfLogger", perfLogger))
                .WithParameter(new NamedParameter("usageLogger", usageLogger))
                .WithParameter(new NamedParameter("errorLogger", errorLogger))
                .WithParameter(new NamedParameter("diagnosticLogger", diagnosticLogger))
                .SingleInstance();

            builder.RegisterFilterProvider();
        }

         */
    }
}
