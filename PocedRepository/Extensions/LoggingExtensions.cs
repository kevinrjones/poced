using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;

namespace PocedRepository.Extensions
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
