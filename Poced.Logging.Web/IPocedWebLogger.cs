// ****************************************************************************
// * Copyright....: ICSA Software International Limited, Copyright (c) 2016
// *
// * Module.......: "iPocedweblogger.cs"
// *
// * Description..:
// * Date.........: 09/01/2018
// * Author.......: Developer
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Poced.Logging.Web
{
    /// <summary> Interface for Poced web logger.</summary>
    public interface IPocedWebLogger : IPocedLogger
    {
        /// <summary> Writes an usage.</summary>
        ///
        /// <param name="context">        The context.</param>
        /// <param name="product">        The product.</param>
        /// <param name="layer">          The layer.</param>
        /// <param name="activityName">   Name of the activity.</param>
        /// <param name="additionalInfo"> Information describing the additional.</param>
        /// <param name="level">          The level.</param>
        void WriteUsage(HttpContext context, string product, string layer, string activityName,
            Dictionary<string, object> additionalInfo = null, LogLevel level = LogLevel.Information);

        /// <summary> Writes an usage.</summary>
        ///
        /// <param name="context">        The context.</param>
        /// <param name="product">        The product.</param>
        /// <param name="layer">          The layer.</param>
        /// <param name="activityName">   Name of the activity.</param>
        /// <param name="location">       The location.</param>
        /// <param name="additionalInfo"> Information describing the additional.</param>
        /// <param name="level">          The level.</param>
        void WriteUsage(HttpContext context, string product, string layer, string activityName, string location,
            Dictionary<string, object> additionalInfo = null, LogLevel level = LogLevel.Information);

        /// <summary> Writes an error.</summary>
        ///
        /// <param name="context"> The context.</param>
        /// <param name="product"> The product.</param>
        /// <param name="layer">   The layer.</param>
        /// <param name="ex">      The ex.</param>
        /// <param name="level">   The level.</param>
        void WriteError(HttpContext context, string product, string layer, Exception ex, LogLevel level = LogLevel.Information);

        /// <summary> Writes an error.</summary>
        ///
        /// <param name="context">       The context.</param>
        /// <param name="product">       The product.</param>
        /// <param name="layer">         The layer.</param>
        /// <param name="correlationId"> Identifier for the correlation.</param>
        /// <param name="ex">            The ex.</param>
        /// <param name="level">         The level.</param>
        void WriteError(HttpContext context, string product, string layer, string correlationId, Exception ex, LogLevel level = LogLevel.Information);

        /// <summary> Writes a diagnostic.</summary>
        ///
        /// <param name="context">        The context.</param>
        /// <param name="product">        The product.</param>
        /// <param name="layer">          The layer.</param>
        /// <param name="message">        The message.</param>
        /// <param name="additionalInfo"> Information describing the additional.</param>
        /// <param name="level">          The level.</param>
        void WriteDiagnostic(HttpContext context, string product, string layer, string message,
            Dictionary<string, object> additionalInfo = null, LogLevel level = LogLevel.Information);

        /// <summary> Gets web logging data.</summary>
        ///
        /// <param name="context">  The context.</param>
        /// <param name="userId">   Identifier for the user.</param>
        /// <param name="userName"> Name of the user.</param>
        /// <param name="location"> The location.</param>
        ///
        /// <returns> The web logging data.</returns>
        Dictionary<string, object> GetWebLoggingData(HttpContext context, out string userId, out string userName, out string location);

        /// <summary> Gets user data.</summary>
        ///
        /// <param name="data">     The data.</param>
        /// <param name="user">     The user.</param>
        /// <param name="userId">   Identifier for the user.</param>
        /// <param name="userName"> Name of the user.</param>
        void GetUserData(Dictionary<string, object> data, ClaimsPrincipal user, out string userId, out string userName);

        /// <summary> Gets location for API call.</summary>
        ///
        /// <param name="httpContext">   Context for the HTTP.</param>
        /// <param name="routeTemplate"> The route template.</param>
        /// <param name="dict">          The dictionary.</param>
        /// <param name="location">      The location.</param>
        void GetLocationAndDataForApiCall(HttpContext httpContext, string routeTemplate, Dictionary<string, object> dict, out string location);
    }
}