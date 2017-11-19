using System;
using System.Collections.Generic;
using Poced.Configuration;
using Serilog;
using Serilog.Events;

namespace Poced.Logging
{
    public class PocedSerlogLogger : IPocedLogger
    {
        private readonly Dictionary<LogLevel, LogEventLevel> _levelMap;

        private readonly ILogger _perfLogger;
        private readonly ILogger _usageLogger;
        private readonly ILogger _errorLogger;
        private readonly ILogger _diagnosticLogger;
        private readonly IConfiguration _configuration;

        public PocedSerlogLogger(ILogger perfLogger,
                           ILogger usageLogger,
                           ILogger errorLogger,
                           ILogger diagnosticLogger,
                           IConfiguration configuration)
        {
            _perfLogger = perfLogger;
            _usageLogger = usageLogger;
            _errorLogger = errorLogger;
            _diagnosticLogger = diagnosticLogger;
            _configuration = configuration;

            _levelMap = new Dictionary<LogLevel, LogEventLevel>
            {
                {LogLevel.Debug, LogEventLevel.Debug},
                {LogLevel.Error, LogEventLevel.Error},
                {LogLevel.Fatal, LogEventLevel.Fatal},
                {LogLevel.Information, LogEventLevel.Information},
                {LogLevel.Verbose, LogEventLevel.Verbose},
                {LogLevel.Warning, LogEventLevel.Warning},
            };
        }

        public void WriteUsage(LogDetail detail, LogLevel level = LogLevel.Information)
        {
            _usageLogger.Write(_levelMap[level], "{@LogDetail}", detail);
        }

        public void WriteError(LogDetail detail, LogLevel level = LogLevel.Information)
        {
            if (detail.Exception != null)
            {
                detail.Message = GetMessageFromException(detail.Exception);
            }
            _errorLogger.Write(_levelMap[level], "{@LogDetail}", detail);
        }

        public void WriteDiagnostic(LogDetail detail, LogLevel level = LogLevel.Information)
        {
            bool writeDiagnostics;
            bool.TryParse(_configuration.GetConfigurationValue("WriteDiagnostics"), out writeDiagnostics);
            if (writeDiagnostics)
            {
                _diagnosticLogger.Write(_levelMap[level], "{@LogDetail}", detail);
            }
        }

        public void WritePerf(LogDetail detail, LogLevel level = LogLevel.Information)
        {
            _perfLogger.Write(_levelMap[level], "{@LogDetail}", detail);
        }

        /// <summary>
        /// Always get the inner most exception message to use as the log
        /// message in the cases of exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetMessageFromException(ex.InnerException);

            return ex.Message;
        }
    }
}
