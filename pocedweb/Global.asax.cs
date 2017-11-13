using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using DataInterfaces;
using FileBasedData;
using PocedRepository;

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
            var connectionString = ConfigurationManager.ConnectionStrings["pocedEntities"].ConnectionString;
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var repositoryAssemblies = Assembly.Load("ArticlesService");
            builder.RegisterAssemblyTypes(repositoryAssemblies).AsImplementedInterfaces();
            builder.RegisterType<ArticlesData>().As<IArticlesData>();

            builder.RegisterType<UsersRepository>().As<IUsersRepository>().WithParameter(new NamedParameter("connectionString", connectionString));            
            builder.RegisterType<ArticlesRepository>().As<IArticlesRepository>().WithParameter(new NamedParameter("connectionString", connectionString));

            builder.RegisterFilterProvider();
        }
    }
}
