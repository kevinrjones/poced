using System.Linq;
using System.Web.Mvc;

namespace Poced.Logging.Web.Attributes
{
    /// <summary>
    /// See http://blog.ploeh.dk/2014/06/13/passive-attributes/
    /// for the idea of 'Passive Attributes'
    /// </summary>
    public class TrackPerformanceFilter : IActionFilter, IResultFilter
    {
        private readonly IPocedLogger _logger;

        public TrackPerformanceFilter(IPocedLogger logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TrackPerformanceAttribute attribute = filterContext
                .ActionDescriptor
                .GetCustomAttributes(typeof(TrackPerformanceAttribute), true)
                .SingleOrDefault() as TrackPerformanceAttribute;

            if (attribute == null)
            {
                attribute = filterContext
                    .ActionDescriptor
                    .ControllerDescriptor
                    .GetCustomAttributes(typeof(TrackPerformanceAttribute), true)
                    .SingleOrDefault() as TrackPerformanceAttribute;

            }

            if (attribute == null) return;


            var dict = Helpers.GetWebFloggingData(out var userId, out var userName, out var location);

            var type = filterContext.HttpContext.Request.RequestType;
            var perfName = filterContext.ActionDescriptor.ActionName + "_" + type;

            var stopwatch = new PerfTracker(_logger, perfName, userId, userName, location,
                attribute.ProductName, attribute.LayerName, dict);
            filterContext.HttpContext.Items["Stopwatch"] = stopwatch;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {

        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var stopwatch = (PerfTracker)filterContext.HttpContext.Items["Stopwatch"];
            stopwatch?.Stop();
        }
    }
}