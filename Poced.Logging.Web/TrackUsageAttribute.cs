// ****************************************************************************
// * Copyright....: ICSA Software International Limited, Copyright (c) 2016
// *
// * Module.......: "trackusageattribute.cs"
// *
// * Description..:
// * Date.........: 09/01/2018
// * Author.......: Developer
// ****************************************************************************

using System;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Poced.Shared;

namespace Poced.Logging.Web
{
    /// <summary>Attribute for tracking usage.
    ///  Use like this
    /// 
    /// [TypeFilter(typeof(TrackUsageAttribute), Arguments = new object[]{"... name of usage... or
    /// empty string"})]
    /// ... code here ...</summary>
    ///
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute"/>
     
    // todo: All action filters OnResultExecuted?
    public class TrackUsageAttribute : ActionFilterAttribute
    {
        /// <summary> The name.</summary>
        private string name;
        /// <summary> The logger.</summary>
        private readonly IPocedWebLogger logger;
        /// <summary> Options for controlling the operation.</summary>
        private readonly PocedLoggingSettings settings;

        /// <summary>Initializes a new instance of the Diligent.Poced.Logging.Web.TrackUsageAttribute
        /// class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="options"> Options for controlling the operation.</param>
        /// <param name="logger">  The logger.</param>
        /// <param name="name">    The name.</param>
        public TrackUsageAttribute(IOptions<PocedLoggingSettings> options, IPocedWebLogger logger, string name)
        {
            this.name = name;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary> Executes the result executed action.</summary>
        ///
        /// <param name="context"> The context.</param>
        ///
        /// <seealso cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnResultExecuted(ResultExecutedContext)"/>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (string.IsNullOrEmpty(name))
            {
                var type = context.HttpContext.Request.Method;
                var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
                name = descriptor?.ControllerName + "_" + descriptor?.ActionName + "_" + type;
            }
            logger.WriteUsage(context.HttpContext, settings.ProductName, settings.LayerName, name);
        }
    }
}