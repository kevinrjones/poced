using System;
using System.Collections.Generic;

namespace Poced.Logging
{
    public class LogDetail
    {
        public LogDetail()
        {
            Timestamp = DateTime.Now;
        }

        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string Product { get; set; }
        public string Layer { get; set; }
        public string Location { get; set; }
        public string Hostname { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public long? EllapsedMilliseconds { get; set; }
        public Exception Exception { get; set; }
        public string CorreletaionId { get; set; }
        public Dictionary<string,object> AdditionalInfo { get; set; }
        public string CorrelationId { get; set; }
    }
}
