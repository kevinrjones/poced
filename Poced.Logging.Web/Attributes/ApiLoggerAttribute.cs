using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Poced.Logging.Web.Attributes
{
    public class ApiLoggerAttribute : ActionFilterAttribute
    {
        public string ProductName { get; }
        public IPocedLogger Logger;


        public ApiLoggerAttribute(string productName)
        {
            ProductName = productName;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
        }
    }
}
