using System;
using Logging;

namespace PocedWeb.Logging
{
    public static class LoggingExtensions
    {
        public static LogDetail CreateFlogDetail(string message, string serviceName, Exception ex)
        {
            return new LogDetail
            {
                Product = "Poced",
                Location = serviceName,
                Layer = "Web",
                UserName = Environment.UserName,
                Hostname = Environment.MachineName,
                Message = message,
                Exception = ex
            };
        }
    }
}
