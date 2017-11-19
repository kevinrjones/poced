using System.Web.Mvc;

namespace Poced.Logging.Web.Attributes
{
    public class TrackPerformanceAttribute : ActionFilterAttribute
    {
        public string LayerName { get; set; }
        public string ProductName { get; set; }

        public IPocedLogger Logger;
        private readonly string _productName;
        private readonly string _layerName;

        // can use like [TrackPerformance("ToDos", "Mvc")]
        public TrackPerformanceAttribute(string product, string layer)
        {
            _productName = product;
            _layerName = layer;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}
