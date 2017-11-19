using System.Web.Mvc;
using Poced.Logging;
using Poced.Logging.Web.Attributes;

namespace Poced.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, IPocedLogger logger)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new TrackUsageFilter(logger));
            filters.Add(new TrackPerformanceFilter(logger));
        }
    }
}
