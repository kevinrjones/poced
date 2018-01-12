using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Poced.Shared;
//using Diligent.Poced.Shared.Config;

namespace Poced.Logging
{
    public class PocedSerilogLogger : IPocedLogger
    {
        private readonly Dictionary<LogLevel, LogEventLevel> levelMap;

        private readonly ILogger perfLogger;
        private readonly ILogger usageLogger;
        private readonly ILogger errorLogger;
        private readonly ILogger diagnosticLogger;
        protected readonly Poced.Shared.PocedLoggingSettings Settings;

        public PocedSerilogLogger(ILogger perfLogger,
            ILogger usageLogger,
            ILogger errorLogger,
            ILogger diagnosticLogger,
            IOptions<PocedLoggingSettings> options)
        {
            this.perfLogger = perfLogger ?? throw new ArgumentNullException(nameof(perfLogger));
            this.usageLogger = usageLogger ?? throw new ArgumentNullException(nameof(usageLogger));
            this.errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
            this.diagnosticLogger = diagnosticLogger ?? throw new ArgumentNullException(nameof(diagnosticLogger));
            Settings = options?.Value ?? throw new ArgumentNullException(nameof(options));


            levelMap = new Dictionary<LogLevel, LogEventLevel>
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
            usageLogger.Write(levelMap[level], "{@LogDetail}", detail);
        }

        public void WriteError(LogDetail detail, LogLevel level = LogLevel.Information)
        {
            if (detail.Exception != null)
            {
                detail.Message = GetMessageFromException(detail.Exception);
            }
            errorLogger.Write(levelMap[level], "{@LogDetail}", detail);
        }

        public void WriteDiagnostic(LogDetail detail, LogLevel level = LogLevel.Information)
        {
            if (Settings.WriteDiagnostics)
            {
                diagnosticLogger.Write(levelMap[level], "{@LogDetail}", detail);
            }
        }

        public void WritePerf(LogDetail detail, LogLevel level = LogLevel.Information)
        {
            perfLogger.Write(levelMap[level], "{@LogDetail}", detail);
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