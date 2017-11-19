namespace Poced.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPocedLogger
    {
        void WriteUsage(LogDetail detail, LogLevel level = LogLevel.Information);
        void WriteError(LogDetail detail, LogLevel level = LogLevel.Information);
        void WriteDiagnostic(LogDetail detail, LogLevel level = LogLevel.Information);
        void WritePerf(LogDetail detail, LogLevel level = LogLevel.Information);
    }

}