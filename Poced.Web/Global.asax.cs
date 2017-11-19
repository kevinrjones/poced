using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Poced.Configuration;
using Poced.Logging;
using Poced.Logging.Web;
using Poced.Repository;
using Poced.Web.Controllers;
using Serilog;

namespace Poced.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        IContainer container;

        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            RegisterTypes(builder);
            container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            var logger = container.Resolve<IPocedLogger>();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, logger);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex == null)
                return;

            var logger = container.Resolve<IPocedLogger>();

            string errorControllerAction;

            Helpers.GetHttpStatus(ex, out var httpStatus);
            switch (httpStatus)
            {
                case 404:
                    errorControllerAction = "NotFound";
                    break;
                default:
                    logger.LogWebError(Constants.ProductName, Constants.Layer, ex);
                    errorControllerAction = "Index";
                    break;
            }

            var httpContext = ((MvcApplication)sender).Context;
            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = httpStatus;
            httpContext.Response.TrySkipIisCustomErrors = true;

            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = errorControllerAction;

            var controller = new ErrorController();
            ((IController)controller)
                .Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }

        private void RegisterTypes(ContainerBuilder builder)
        {
            var perfLogger = new LoggerConfiguration().CreateLogger();
            var usageLogger = new LoggerConfiguration().CreateLogger();
            var errorLogger = new LoggerConfiguration().CreateLogger();
            var diagnosticLogger = new LoggerConfiguration().CreateLogger();

            var connectionString = ConfigurationManager.ConnectionStrings["pocedEntities"].ConnectionString;
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var repositoryAssemblies = Assembly.Load("Poced.Services");
            builder.RegisterAssemblyTypes(repositoryAssemblies).AsImplementedInterfaces();
//            builder.RegisterType<ArticlesData>().As<IArticlesData>();

            //builder.RegisterType<UsersRepository>().As<IUsersRepository>().WithParameter(new NamedParameter("connectionString", connectionString));            
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
    }
}
