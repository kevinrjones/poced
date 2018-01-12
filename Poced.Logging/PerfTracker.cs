// ****************************************************************************
// * Copyright....: ICSA Software International Limited, Copyright (c) 2016
// *
// * Module.......: "PerfTracker.cs"
// *
// * Description..:
// * Date.........: 11/01/2018
// * Author.......: Developer
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Poced.Logging
{
    /// <summary> A performance tracker.</summary>
    ///
    /// <seealso cref="T:System.IDisposable"/>
    public class PerfTracker : IDisposable

    {
        /// <summary> The logger.</summary>
        private readonly IPocedLogger logger;
        /// <summary> The software.</summary>
        private readonly Stopwatch sw;
        /// <summary> The information to log.</summary>
        private readonly LogDetail infoToLog;

        /// <summary>Initializes a new instance of the Diligent.Poced.Logging.PerfTracker class.</summary>
        ///
        /// <param name="logger">   The logger.</param>
        /// <param name="name">     The name.</param>
        /// <param name="userId">   Identifier for the user.</param>
        /// <param name="userName"> Name of the user.</param>
        /// <param name="product">  The product.</param>
        /// <param name="location"> The location.</param>
        /// <param name="layer">    The layer.</param>
        public PerfTracker(IPocedLogger logger, string name, string userId, string userName,
            string product, string location, string layer)
        {
            this.logger = logger;
            sw = Stopwatch.StartNew();
            infoToLog = new LogDetail
            {
                Message = name,
                UserId = userId,
                UserName = userName,
                Product = product,
                Layer = layer,
                Location = location,
                Hostname = Environment.MachineName
            };

            var beginTime = DateTime.Now;
            infoToLog.AdditionalInfo = new Dictionary<string, object>
                    {{"Started", beginTime.ToString(CultureInfo.InvariantCulture)}};
        }

        /// <summary>Initializes a new instance of the Diligent.Poced.Logging.PerfTracker class.</summary>
        ///
        /// <param name="logger">     The logger.</param>
        /// <param name="name">       The name.</param>
        /// <param name="userId">     Identifier for the user.</param>
        /// <param name="userName">   Name of the user.</param>
        /// <param name="product">    The product.</param>
        /// <param name="location">   The location.</param>
        /// <param name="layer">      The layer.</param>
        /// <param name="perfParams"> Options for controlling the performance.</param>
        public PerfTracker(IPocedLogger logger, string name, string userId, string userName,
            string product, string location, string layer, Dictionary<string, object> perfParams)
            : this(logger, name, userId, userName, product, location, layer)
        {
            foreach (var item in perfParams)
            {
                infoToLog.AdditionalInfo.Add("input-" + item.Key, item.Value);
            }
        }

        /// <summary> Stops this object.</summary>
        public void Stop()
        {
            sw.Stop();
            infoToLog.EllapsedMilliseconds = sw.ElapsedMilliseconds;
            logger.WritePerf(infoToLog);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.</summary>
        ///
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        public void Dispose()
        {
            Stop();
        }
    }
}

