using CommandLine;

namespace ADDSMock.Applications.Configuration
{
    internal class CommandLineOptions
    {
        [Option('i', "interactive", Required = false, HelpText = "Run ADDSMock in the Interactive mode")] public bool IsInteractive { get; set; }
    }
}
