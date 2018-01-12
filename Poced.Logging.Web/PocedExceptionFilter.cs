using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Poced.Shared;

namespace Poced.Logging.Web
{
    /// <summary> An exception filter.
    ///           Use this by calling
    ///           
    ///    services.AddMvc(options =>
    ///    {
    ///        options.Filters.Add&lt;PocedExceptionFilter>();
    ///    });
    ///
    /// </summary>
    ///
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter"/>
    public class PocedExceptionFilter : IExceptionFilter
    {
        private readonly IPocedWebLogger logger;
        private readonly PocedLoggingSettings settings;

        /// <summary>Initializes a new instance of the
        /// Diligent.Poced.Logging.Web.PocedExceptionFilter class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="options"> Options for controlling the operation.</param>
        /// <param name="logger">  The logger.</param>
        public PocedExceptionFilter(IOptions<PocedLoggingSettings> options, IPocedWebLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary> Called after an action has thrown an <see cref="T:System.Exception" />.</summary>
        ///
        /// <param name="exceptionContext">The
        /// <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext" />.</param>
        ///
        /// <seealso cref="M:Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter.OnException(ExceptionContext)"/>
        public void OnException(ExceptionContext exceptionContext)
        {
            string correlationId = Guid.NewGuid().ToString();
            logger.WriteError(exceptionContext.HttpContext, settings.ProductName, settings.LayerName, correlationId, exceptionContext.Exception);

            HttpStatusCode status;
            String message = $"Something went wrong. Please contact our support team referencing {correlationId}\n";

            var exceptionType = exceptionContext.Exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message += "Unauthorized Access";
                status = HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(NotImplementedException))
            {
                message += "A server error occurred.";
                status = HttpStatusCode.NotImplemented;
            }
            else if (exceptionType == typeof(PocedException))
            {
                message += exceptionContext.Exception.Message;
                status = HttpStatusCode.InternalServerError;
            }
            else
            {
                message += exceptionContext.Exception.Message;
                status = HttpStatusCode.NotFound;
            }
            HttpResponse response = exceptionContext.HttpContext.Response;
            response.StatusCode = (int)status;
            response.ContentType = "application/json";
            var err = message + " " + exceptionContext.Exception.StackTrace;
            response.WriteAsync(err);
        }
    }
}