using Serilog;

namespace Poced.Logging
{
    public interface ISerilogPerfLogger : ILogger { }
    public interface ISerilogUsageLogger : ILogger { }
    public interface ISerilogDiagnosticLogger : ILogger { }
    public interface ISerilogErrorLogger : ILogger { }
}