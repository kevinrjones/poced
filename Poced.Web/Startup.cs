using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poced.Identity.Shared;
using Poced.Logging;
using Poced.Logging.Web;
using Poced.Repository;
using Poced.Repository.Contexts;
using Poced.Shared;
using Serilog;

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

            services.Configure<PocedApiSettings>(Configuration.GetSection("PocedApiSettings"));
            services.Configure<PocedLoggingSettings>(Configuration.GetSection("PocedLoggingSettings"));

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

            // Create the container builder.
            var builder = new ContainerBuilder();

            
            builder.Populate(services);

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
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });

            services.AddMvc(options =>
            {
                options.Filters.Add<TrackPerformanceAttribute>();
                options.Filters.Add<PocedExceptionFilter>();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var repositoryAssemblies = Assembly.Load("Poced.Services");
            builder.RegisterAssemblyTypes(repositoryAssemblies).AsImplementedInterfaces();

            builder.RegisterType<ArticlesRepository>().As<IArticlesRepository>();
            builder.RegisterType<UsersRepository>().As<IUsersRepository>();

            ConfigureSerilogLogger(builder);
        }

        private void ConfigureSerilogLogger(ContainerBuilder builder)
        {
            var perfLogger = new LoggerConfiguration()
                .WriteTo.File("web-log-perf.txt")
                .CreateLogger();
            var diagLogger = new LoggerConfiguration()
                .WriteTo.File("web-log-diag.txt")
                .CreateLogger();
            var usageLogger = new LoggerConfiguration()
                .WriteTo.File("web-log-usage.txt")
                .CreateLogger();
            var errorLogger = new LoggerConfiguration()
                .WriteTo.File("web-log-error.txt")
                .CreateLogger();

            var perfLogParam = new NamedParameter("perfLogger", perfLogger);
            var diagLogParam = new NamedParameter("diagnosticLogger", diagLogger);
            var usageLogParam = new NamedParameter("usageLogger", usageLogger);
            var errorLogParam = new NamedParameter("errorLogger", errorLogger);

            builder.RegisterType<PocedSerilogWebLogger>()
                .As<IPocedWebLogger>()
                .WithParameter(perfLogParam)
                .WithParameter(diagLogParam)
                .WithParameter(usageLogParam)
                .WithParameter(errorLogParam);

            builder.RegisterType<PocedSerilogLogger>()
                .As<IPocedLogger>()
                .WithParameter(perfLogParam)
                .WithParameter(diagLogParam)
                .WithParameter(usageLogParam)
                .WithParameter(errorLogParam);
        }
        
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
    }
}
