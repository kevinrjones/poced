using System;
using Poced.Logging;

namespace Poced.Repository.Extensions
{
    public static class LoggingExtensions
    {
        public static LogDetail CreateFlogDetail(string message, string serviceName, Exception ex)
        {
            return new LogDetail
            {
                Product = "Poced",
                Location = serviceName,
                Layer = "Repository",
                UserName = Environment.UserName,
                Hostname = Environment.MachineName,
                Message = message,
                Exception = ex
            };
        }
    }
}
