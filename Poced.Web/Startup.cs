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
using Serilog.Sinks.SystemConsole.Themes;

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

            // Create the container builder.
            var builder = new ContainerBuilder();

            
            builder.Populate(services);

                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "oidc.cookie";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("oidc.cookie")
                //.AddGoogle(options =>
                //{
                //    options.ClientId = Configuration["Authentication:Google:ClientId"];
                //    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                //})
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "http://poced.ids.local";
                    options.ClientId = "poced";
                    options.SignInScheme = "oidc.cookie";
                    //options.ClientSecret = "nKsxtdRxowzf0AqYvd6maiex6pdxLTml3AN+vw9sQp+9S8pnGVPOXWjMcfoN2cMXGrDJ7R82UMtNMvosHN53dg==";
                    options.RequireHttpsMetadata = false;
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
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();
            var diagLogger = new LoggerConfiguration()
                .WriteTo.File("web-log-diag.txt")
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();
            var usageLogger = new LoggerConfiguration()
                .WriteTo.File("web-log-usage.txt")
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();
            var errorLogger = new LoggerConfiguration()
                .WriteTo.File("web-log-error.txt")
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
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
                app.UseExceptionHandler("/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Articles}/{action=Index}/{id?}");
            //});
        }
    }
}
