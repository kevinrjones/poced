using System.Web.Mvc;
using System.Linq;

namespace Poced.Logging.Web.Attributes
{
    /// <summary>
    /// See http://blog.ploeh.dk/2014/06/13/passive-attributes/
    /// for the idea of 'Passive Attributes'
    /// </summary>
    public class TrackUsageFilter : IActionFilter
    {
        private IPocedLogger _logger;

        public TrackUsageFilter(IPocedLogger logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            TrackUsageAttribute attribute = filterContext
                .ActionDescriptor
                .GetCustomAttributes(typeof(TrackUsageAttribute), true)
                .SingleOrDefault() as TrackUsageAttribute;

            if (attribute == null)
            {
                attribute = filterContext
                    .ActionDescriptor
                    .ControllerDescriptor
                    .GetCustomAttributes(typeof(TrackUsageAttribute), true)
                    .SingleOrDefault() as TrackUsageAttribute;
            }

            if (attribute == null) return;

            _logger.LogWebUsage(attribute.Product, attribute.Layer, attribute.Name);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}