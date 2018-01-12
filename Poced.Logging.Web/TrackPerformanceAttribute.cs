// ****************************************************************************
// * Copyright....: ICSA Software International Limited, Copyright (c) 2016
// *
// * Module.......: "trackperformanceattribute.cs"
// *
// * Description..:
// * Date.........: 09/01/2018
// * Author.......: Kevin Jones
// ****************************************************************************

using System;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Poced.Shared;

namespace Poced.Logging.Web
{
    /// <summary>Attribute to track performance in an MVC application. Use like this
    /// 
    /// services.AddMvc(options =>
    /// {
    /// options.Filters.Add&lt;TrackPerformanceAttribute>();
    /// });
    /// 
    /// or
    /// 
    /// [TypeFilter(typeof(TrackPerformanceAttribute))]
    /// ... code here ...</summary>
    ///
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute"/>
    public class TrackPerformanceAttribute : ActionFilterAttribute
    {
        /// <summary> The logger.</summary>
        private readonly IPocedWebLogger logger;
        /// <summary> Options for controlling the operation.</summary>
        private readonly PocedLoggingSettings settings;

        /// <summary>Initializes a new instance of the
        /// Diligent.Poced.Logging.Web.TrackPerformanceAttribute class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="options"> Options for controlling the operation.</param>
        /// <param name="logger">  The logger.</param>
        public TrackPerformanceAttribute(IOptions<PocedLoggingSettings> options, IPocedWebLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary> Executes the action executing action.</summary>
        ///
        /// <param name="filterContext"> Context for the filter.</param>
        ///
        /// <seealso cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnActionExecuting(ActionExecutingContext)"/>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var dict = logger.GetWebLoggingData(filterContext.HttpContext, out var userId, out var userName, out var location);

            var type = filterContext.HttpContext.Request.Method;
            var perfName = (filterContext.ActionDescriptor as ControllerActionDescriptor)?.ActionName + "_" + type;

            var stopwatch = new PerfTracker(logger, perfName, userId, userName, location, settings.ProductName, settings.LayerName, dict);
            filterContext.HttpContext.Items["Stopwatch"] = stopwatch;
        }

        /// <summary> Executes the result executed action.</summary>
        ///
        /// <param name="filterContext"> Context for the filter.</param>
        ///
        /// <seealso cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnResultExecuted(ResultExecutedContext)"/>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var stopwatch = (PerfTracker)filterContext.HttpContext.Items["Stopwatch"];
            stopwatch?.Stop();
        }
    }
}