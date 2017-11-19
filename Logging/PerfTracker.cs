using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class PerfTracker : IDisposable

    {
        private readonly IPocedLogger _logger;
        private Stopwatch _sw;
        private readonly LogDetail _infoToLog;

        public PerfTracker(IPocedLogger logger, string name, string userId, string userName,
            string product, string location, string layer)
        {
            _logger = logger;
            _sw = Stopwatch.StartNew();
            _infoToLog = new LogDetail
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
            _infoToLog.AdditionalInfo = new Dictionary<string, object>
            {{"Started", beginTime.ToString(CultureInfo.InvariantCulture)}};
        }


        public PerfTracker(IPocedLogger logger, string name, string userId, string userName,
            string product, string location, string layer, Dictionary<string, object> perfParams)
            : this(logger, name, userId, userName, product, location, layer)
        {
            foreach (var item in perfParams)
            {
                _infoToLog.AdditionalInfo.Add("input-" + item.Key, item.Value);
            }
        }

        protected void Stop()
        {
            _sw.Stop();
            _infoToLog.EllapsedMilliseconds = _sw.ElapsedMilliseconds;
            _logger.WritePerf(_infoToLog);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
