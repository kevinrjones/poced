using System.Web.Mvc;

namespace Poced.Logging.Web.Attributes
{
    //requires NuGet package Microsoft.AspNet.Mvc
    public class TrackUsageAttribute : ActionFilterAttribute
    {
        public string Product { get; }
        public string Layer { get; }
        public string Name { get; }

        public TrackUsageAttribute(string product, string layer, string name)
        {
            Product = product;
            Layer = layer;
            Name = name;
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}
