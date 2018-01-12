// ****************************************************************************
// * Copyright....: ICSA Software International Limited, Copyright (c) 2016
// *
// * Module.......: "Pocedserilogweblogger.cs"
// *
// * Description..:
// * Date.........: 09/01/2018
// * Author.......: Developer
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Poced.Shared;
using Serilog;
using UAParser;

namespace Poced.Logging.Web
{
    /// <summary> A Poced serilog web logger.</summary>
    ///
    /// <seealso cref="T:Diligent.Poced.Logging.PocedSerilogLogger"/>
    /// <seealso cref="T:Diligent.Poced.Logging.Web.IPocedWebLogger"/>
    public class PocedSerilogWebLogger : PocedSerilogLogger, IPocedWebLogger
    {
        /// <summary>Initializes a new instance of the
        /// Diligent.Poced.Logging.Web.PocedSerilogWebLogger class.</summary>
        ///
        /// <param name="perfLogger">       The performance logger.</param>
        /// <param name="usageLogger">      The usage logger.</param>
        /// <param name="errorLogger">      The error logger.</param>
        /// <param name="diagnosticLogger"> The diagnostic logger.</param>
        /// <param name="options">          Options for controlling the operation.</param>
        public PocedSerilogWebLogger(ILogger perfLogger, ILogger usageLogger, ILogger errorLogger, ILogger diagnosticLogger, IOptions<PocedLoggingSettings> options) 
            : base(perfLogger, usageLogger, errorLogger, diagnosticLogger, options)
        {
        }

        /// <summary> Writes a usage log message.</summary>
        ///
        /// <param name="context">        The context.</param>
        /// <param name="product">        The product.</param>
        /// <param name="layer">          The layer.</param>
        /// <param name="activityName">   Name of the activity.</param>
        /// <param name="location">       The location.</param>
        /// <param name="additionalInfo"> Information describing the additional.</param>
        /// <param name="level">          The level.</param>
        ///
        /// <seealso cref="M:Diligent.Poced.Logging.Web.IPocedWebLogger.WriteUsage(HttpContext,string,string,string,string,Dictionary{string,object},LogLevel)"/>
        public void WriteUsage(HttpContext context, string product, string layer, string activityName, string location, Dictionary<string, object> additionalInfo = null, LogLevel level = LogLevel.Information)
        {
            var webInfo = GetWebLoggingData(context, out var userId, out var userName, out var ignore);

            var usageInfo = GenerateUsageInfo(context, product, layer, activityName, additionalInfo, webInfo, location, userId, userName);
            WriteUsage(usageInfo);
        }

        /// <summary> Writes n usage.</summary>
        ///
        /// <param name="context">        The context.</param>
        /// <param name="product">        The product.</param>
        /// <param name="layer">          The layer.</param>
        /// <param name="activityName">   Name of the activity.</param>
        /// <param name="additionalInfo"> Information describing the additional.</param>
        /// <param name="level">          The level.</param>
        ///
        /// <seealso cref="M:Diligent.Poced.Logging.Web.IPocedWebLogger.WriteUsage(HttpContext,string,string,string,Dictionary{string,object},LogLevel)"/>
        public void WriteUsage(HttpContext context, string product, string layer, string activityName, Dictionary<string, object> additionalInfo = null, LogLevel level = LogLevel.Information)
        {
            var webInfo = GetWebLoggingData(context, out var userId, out var userName, out var location);

            var usageInfo = GenerateUsageInfo(context, product, layer, activityName, additionalInfo, webInfo, location, userId, userName);
            WriteUsage(usageInfo);
        }

        /// <summary> Writes an error.</summary>
        ///
        /// <param name="context"> The context.</param>
        /// <param name="product"> The product.</param>
        /// <param name="layer">   The layer.</param>
        /// <param name="ex">      The ex.</param>
        /// <param name="level">   The level.</param>
        ///
        /// <seealso cref="M:Diligent.Poced.Logging.Web.IPocedWebLogger.WriteError(HttpContext,string,string,Exception,LogLevel)"/>
        public void WriteError(HttpContext context, string product, string layer, Exception ex, LogLevel level = LogLevel.Information)
        {
            var webInfo = GetWebLoggingData(context, out var userId, out var userName, out var location);

            var errorInformation = new LogDetail()
            {
                Product = product,
                Layer = layer,
                Location = location,
                UserId = userId,
                UserName = userName,
                Hostname = Environment.MachineName,
                Exception = ex,
                AdditionalInfo = webInfo
            };
            TryAddSessionIdAsCorrelationId(context, errorInformation);
            WriteError(errorInformation);
        }

        /// <summary> Writes an error.</summary>
        ///
        /// <param name="context">       The context.</param>
        /// <param name="product">       The product.</param>
        /// <param name="layer">         The layer.</param>
        /// <param name="correlationId"> Identifier for the correlation.</param>
        /// <param name="ex">            The ex.</param>
        /// <param name="level">         The level.</param>
        ///
        /// <seealso cref="M:Diligent.Poced.Logging.Web.IPocedWebLogger.WriteError(HttpContext,string,string,string,Exception,LogLevel)"/>
        public void WriteError(HttpContext context, string product, string layer, string correlationId, Exception ex, LogLevel level = LogLevel.Information)
        {
            var webInfo = GetWebLoggingData(context, out var userId, out var userName, out var location);

            var errorInformation = new LogDetail()
            {
                Product = product,
                Layer = layer,
                Location = location,
                UserId = userId,
                UserName = userName,
                Hostname = Environment.MachineName,
                Exception = ex,
                CorrelationId = correlationId,
                AdditionalInfo = webInfo
            };
            WriteError(errorInformation);
        }

        /// <summary> Writes a diagnostic.</summary>
        ///
        /// <param name="context">        The context.</param>
        /// <param name="product">        The product.</param>
        /// <param name="layer">          The layer.</param>
        /// <param name="message">        The message.</param>
        /// <param name="additionalInfo"> Information describing the additional.</param>
        /// <param name="level">          The level.</param>
        ///
        /// <seealso cref="M:Diligent.Poced.Logging.Web.IPocedWebLogger.WriteDiagnostic(HttpContext,string,string,string,Dictionary{string,object},LogLevel)"/>
        public void WriteDiagnostic(HttpContext context, string product, string layer, string message, Dictionary<string, object> additionalInfo = null, LogLevel level = LogLevel.Information)
        {
            if (!Settings.WriteDiagnostics) return;

            var webInfo = GetWebLoggingData(context, out var userId, out var userName, out var location);
            if (additionalInfo != null)
            {
                foreach (var key in additionalInfo.Keys)
                    webInfo.Add(key, additionalInfo[key]);
            }

            var diagInfo = new LogDetail()
            {
                Product = product,
                Layer = layer,
                Location = location,
                UserId = userId,
                UserName = userName,
                Hostname = Environment.MachineName,
                Message = message,
                AdditionalInfo = webInfo
            };
            TryAddSessionIdAsCorrelationId(context, diagInfo);
            WriteDiagnostic(diagInfo);
        }

        /// <summary> Gets web logging data.</summary>
        ///
        /// <param name="context">  The context.</param>
        /// <param name="userId">   Identifier for the user.</param>
        /// <param name="userName"> Name of the user.</param>
        /// <param name="location"> The location.</param>
        ///
        /// <returns> The web logging data.</returns>
        ///
        /// <seealso cref="M:Diligent.Poced.Logging.Web.IPocedWebLogger.GetWebLoggingData(HttpContext,out string,out string,out string)"/>
        public Dictionary<string, object> GetWebLoggingData(HttpContext context, out string userId, out string userName, out string location)
        {
            var data = new Dictionary<string, object>();
            location = "unknown";

            GetUserData(data, ClaimsPrincipal.Current, out userId, out userName);

            if(!String.IsNullOrEmpty(context?.Request?.Path)) location = context?.Request?.Path;

            if (!string.IsNullOrEmpty(context?.Request?.Headers?["User-Agent"]))
            {
                GetRequestData(context.Request, data);
            }

            try
            {
                GetSessionData(context.Session, data);
            }
            catch (Exception)
            {
                // fail silently, this does not work if using the WebAPI
            }
            return data;
        }

        /// <summary> Gets location and other other data for API call.</summary>
        ///
        /// <param name="httpContext">   Context for the HTTP.</param>
        /// <param name="routeTemplate"> The route template.</param>
        /// <param name="dict">          The dictionary.</param>
        /// <param name="location">      The location.</param>
        ///
        /// <seealso cref="M:Diligent.Poced.Logging.Web.IPocedWebLogger.GetLocationForApiCall(HttpContext,string,Dictionary{string,object},out string)"/>
        public void GetLocationAndDataForApiCall(HttpContext httpContext, string routeTemplate, Dictionary<string, object> dict, out string location)
        {
            // example route template: api/{controller}/{id}
            var method = httpContext.Request.Method;  // GET, POST, etc.

            foreach (var key in httpContext.GetRouteData().Values.Keys)
            {
                var value = httpContext.GetRouteData().Values[key].ToString();
                if (Int64.TryParse(value, out long _))  // C# 7 inline declaration
                    // must be numeric part of route
                    dict.Add($"Route-{key}", value);
                else
                    routeTemplate = routeTemplate.Replace("{" + key + "}", value);
            }

            location = $"{method} {routeTemplate}";

            var query = httpContext.Request.Query;
            var i = 0;
            foreach (string key in query.Keys)
            {
                var newKey = $"q-{i++}-{key}";
                if (!dict.ContainsKey(newKey))
                    dict.Add(newKey, query[key]);
            }

            StringValues referrer = httpContext.Request.Headers["Referrer"];
            if (referrer.Count != 0)
            {
                var source = referrer.ToString();
                if (source.ToLower().Contains("swagger"))
                    source = "Swagger";
                if (!dict.ContainsKey("Referrer"))
                    dict.Add("Referrer", source);
            }
        }

        /// <summary> Gets user data.</summary>
        ///
        /// <param name="data">     The data.</param>
        /// <param name="user">     The user.</param>
        /// <param name="userId">   Identifier for the user.</param>
        /// <param name="userName"> Name of the user.</param>
        ///
        /// <seealso cref="M:Diligent.Poced.Logging.Web.IPocedWebLogger.GetUserData(Dictionary{string,object},ClaimsPrincipal,out string,out string)"/>
        public void GetUserData(Dictionary<string, object> data, ClaimsPrincipal user, out string userId, out string userName)
        {
            userId = "";
            userName = "";

            if (user != null)
            {
                var i = 1; // i included in dictionary key to ensure uniqueness
                foreach (var claim in user.Claims)
                {
                    if (claim.Type == ClaimTypes.NameIdentifier)
                        userId = claim.Value;
                    else if (claim.Type == ClaimTypes.Name)
                        userName = claim.Value;
                    else
                        // example dictionary key: UserClaim-4-role 
                        data.Add($"UserClaim-{i++}-{claim.Type}", claim.Value);
                }
            }
        }

        /// <summary> Gets request data.</summary>
        ///
        /// <param name="request">  The request.</param>
        /// <param name="requestData">     The data in the request, including the browser, useragent and query string</param>
        private void GetRequestData(HttpRequest request, Dictionary<string, object> requestData)
        {
            if (request != null)
            {
                GetBrowserInfo(request, out var type, out var version);
                requestData.Add("Browser", $"{type}{version}");
                requestData.Add("UserAgent", request.Headers["User-Agent"]);

                foreach (var qsKey in request.Query?.Keys)
                {
                    requestData.Add($"QueryString-{qsKey}", request.Query[qsKey]);
                }
            }
        }

        /// <summary> Gets browser information.</summary>
        ///
        /// <param name="request"> The request.</param>
        /// <param name="type">    The type.</param>
        /// <param name="version"> The version.</param>
        private void GetBrowserInfo(HttpRequest request, out string type, out string version)
        {
            type = "unknown";
            version = "unknown";
            string userAgent = request.Headers["User-Agent"];
            if (!string.IsNullOrEmpty(userAgent))
            {
                var parser = Parser.GetDefault();
                ClientInfo clientInfo = parser.Parse(userAgent);
                type = clientInfo.UserAgent.Family;
                version = " (v " + clientInfo.UserAgent.Major + "." + clientInfo.UserAgent.Minor + ")";
            }
        }

        /// <summary> Gets session data.</summary>
        ///
        /// <param name="session"> The session.</param>
        /// <param name="data">    The data.</param>
        private void GetSessionData(ISession session, Dictionary<string, object> data)
        {
            if (session != null)
            {
                foreach (var key in session.Keys)
                {
                    var keyName = key;
                    if (session.GetString(keyName) != null)
                    {
                        data.Add($"Session-{keyName}",
                            session.GetString(keyName));
                    }
                }
                data.Add("SessionId", session.Id);
            }
        }

        /// <summary> Generates an usage information.</summary>
        ///
        /// <param name="context">        The context.</param>
        /// <param name="product">        The product.</param>
        /// <param name="layer">          The layer.</param>
        /// <param name="activityName">   Name of the activity.</param>
        /// <param name="additionalInfo"> Information describing the additional.</param>
        /// <param name="webInfo">        Information describing the web.</param>
        /// <param name="location">       The location.</param>
        /// <param name="userId">         Identifier for the user.</param>
        /// <param name="userName">       Name of the user.</param>
        ///
        /// <returns> The usage information.</returns>
        private LogDetail GenerateUsageInfo(HttpContext context, string product, string layer, string activityName, Dictionary<string, object> additionalInfo, Dictionary<string, object> webInfo, string location, string userId, string userName)
        {
            if (additionalInfo != null)
            {
                foreach (var key in additionalInfo.Keys)
                    webInfo.Add($"Info-{key}", additionalInfo[key]);
            }

            var usageInfo = new LogDetail()
            {
                Product = product,
                Layer = layer,
                Location = location,
                UserId = userId,
                UserName = userName,
                Hostname = Environment.MachineName,
                Message = activityName,
                AdditionalInfo = webInfo
            };
            TryAddSessionIdAsCorrelationId(context, usageInfo);
            return usageInfo;
        }

        /// <summary> Try add session identifier as correlation identifier.</summary>
        ///
        /// <param name="context">   The context.</param>
        /// <param name="usageInfo"> Information describing the usage.</param>
        private void TryAddSessionIdAsCorrelationId(HttpContext context, LogDetail usageInfo)
        {
            try
            {
                usageInfo.CorrelationId = context.Session.Id;
            }
            catch (Exception)
            {
                // Can't use the session if this is a WebAPI call so create a GUID instead
                usageInfo.CorrelationId = Guid.NewGuid().ToString();
            }
        }

    }
}
