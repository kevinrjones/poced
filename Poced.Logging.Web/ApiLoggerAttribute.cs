// ****************************************************************************
// * Copyright....: ICSA Software International Limited, Copyright (c) 2016
// *
// * Module.......: "apiloggerattribute.cs"
// *
// * Description..:
// * Date.........: 09/01/2018
// * Author.......: Kevin Jones
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Poced.Shared;

namespace Poced.Logging.Web
{
    /// <summary>Attribute to track performance in a WEBAPI application. Use like this
    /// 
    /// services.AddMvc(options =>
    /// {
    /// options.Filters.Add&lt;ApiLoggerAttribute>();
    /// });
    /// 
    /// or
    /// 
    /// [TypeFilter(typeof(TrackPerformanceAttribute))]
    /// ... code here ...</summary>
    ///
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute"/>
    public class ApiLoggerAttribute : ActionFilterAttribute
    {
        /// <summary> The logger.</summary>
        private readonly IPocedWebLogger logger;
        /// <summary> Options for controlling the operation.</summary>
        private readonly PocedLoggingSettings settings;

        /// <summary>Initializes a new instance of the Diligent.Poced.Logging.Web.ApiLoggerAttribute
        /// class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="options"> Options for controlling the operation.</param>
        /// <param name="logger">  The logger.</param>
        public ApiLoggerAttribute(IOptions<PocedLoggingSettings> options, IPocedWebLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary> Executes the action executing action.</summary>
        ///
        /// <param name="actionContext"> Context for the action.</param>
        ///
        /// <seealso cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnActionExecuting(ActionExecutingContext)"/>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            string routeTemplate = actionContext.ActionDescriptor.AttributeRouteInfo.Template;

            var dict = new Dictionary<string, object>();

            ClaimsPrincipal user = actionContext.HttpContext.User;
            logger.GetUserData(dict, user, out var userId, out var userName);

            logger.GetLocationAndDataForApiCall(actionContext.HttpContext, routeTemplate, dict, out var location);

            actionContext.HttpContext.Items["PerfTracker"] = new PerfTracker(logger, location,
                userId, userName, settings.ProductName, location, "API", dict);
        }

        /// <summary> Executes the action executed action.</summary>
        ///
        /// <param name="actionExecutedContext"> Context for the action executed.</param>
        ///
        /// <seealso cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnActionExecuted(ActionExecutedContext)"/>
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            try
            {
                var perfTracker = actionExecutedContext.HttpContext.Items["PerfTracker"]
                    as PerfTracker;

                perfTracker?.Stop();
            }
            catch (Exception)
            {
                // ignoring logging exceptions -- don't want an API call to fail 
                // if we run into logging problems. 
            }
        }
    }
}
