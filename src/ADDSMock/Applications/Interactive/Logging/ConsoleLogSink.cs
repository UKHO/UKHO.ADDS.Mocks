using Serilog;
using Serilog.Configuration;

namespace ADDSMock.Applications.Interactive.Logging
{
    internal static class ConsoleLogSink
    {
        internal static LoggerConfiguration InteractiveConsole(this LoggerSinkConfiguration configuration, ConsoleLog log) => configuration.Sink(log);
    }
}
