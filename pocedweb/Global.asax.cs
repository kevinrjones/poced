using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Configuration;
using DataInterfaces;
using FileBasedData;
using Logging;
using PocedRepository;
using Serilog;

namespace PocedWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            RegisterTypes(builder);
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void RegisterTypes(ContainerBuilder builder)
        {
            var perfLogger = new LoggerConfiguration().CreateLogger();
            var usageLogger = new LoggerConfiguration().CreateLogger();
            var errorLogger = new LoggerConfiguration().CreateLogger();
            var diagnosticLogger = new LoggerConfiguration().CreateLogger();

            var connectionString = ConfigurationManager.ConnectionStrings["pocedEntities"].ConnectionString;
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var repositoryAssemblies = Assembly.Load("PocedServices");
            builder.RegisterAssemblyTypes(repositoryAssemblies).AsImplementedInterfaces();
            builder.RegisterType<ArticlesData>().As<IArticlesData>();

            //builder.RegisterType<UsersRepository>().As<IUsersRepository>().WithParameter(new NamedParameter("connectionString", connectionString));            
            builder.RegisterType<ArticlesRepository>().As<IArticlesRepository>().WithParameter(new NamedParameter("connectionString", connectionString));
            builder.RegisterType<UsersRepository>().As<IUsersRepository>().WithParameter(new NamedParameter("connectionString", connectionString));
            builder.RegisterType<AppConfiguration>().As<IConfiguration>();


            // todo: autofac named parameters with registered values
            builder.RegisterType<PocedSerlogLogger>().As<IPocedLogger>().WithParameter(new NamedParameter("perfLogger", perfLogger))
                .WithParameter(new NamedParameter("usageLogger", usageLogger))
                .WithParameter(new NamedParameter("usageLogger", errorLogger))
                .WithParameter(new NamedParameter("diagnosticLogger", diagnosticLogger));

            builder.RegisterFilterProvider();
        }
    }
}
