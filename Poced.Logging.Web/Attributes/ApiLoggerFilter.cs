using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Poced.Logging.Web.Attributes
{
    /// <summary>
    /// See http://blog.ploeh.dk/2014/06/13/passive-attributes/
    /// for the idea of 'Passive Attributes'
    /// </summary>
    public class ApiLoggerFilter : IActionFilter
    {
        private readonly IPocedLogger _logger;
        public bool AllowMultiple => false;

        public ApiLoggerFilter(IPocedLogger logger)
        {
            _logger = logger;
        }


        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var attribute = actionContext
                .ActionDescriptor
                .GetCustomAttributes<ApiLoggerAttribute>()
                .SingleOrDefault();

            if (attribute == null)
            {
                attribute = actionContext
                    .ControllerContext
                    .ControllerDescriptor
                    .GetCustomAttributes<ApiLoggerAttribute>()
                    .SingleOrDefault();
            }

            if (attribute == null) return await continuation();


            var dict = new Dictionary<string, object>();

            var user = actionContext.RequestContext.Principal as ClaimsPrincipal;
            Helpers.GetUserData(dict, user, out var userId, out var userName);

            actionContext.RequestContext.GetLocationForApiCall(dict, out var location);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            try
            {
                using (PerfTracker perfTracker = new PerfTracker(_logger, location,
                    userId, userName, location, attribute.ProductName, "API", dict))
                {
                    result = await continuation();
                    perfTracker.Stop();
                }
            }
            catch (Exception)
            {
                // ignoring logging exceptions -- don't want an API call to fail 
                // if we run into logging problems. 
            }

            return result;
        }
    }
}