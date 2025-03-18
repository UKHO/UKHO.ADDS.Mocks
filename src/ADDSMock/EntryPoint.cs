using ADDSMock.Applications.Configuration;
using ADDSMock.Applications.Console;
using ADDSMock.Applications.Interactive;
using ADDSMock.Domain.Services;
using Avalonia;
using Avalonia.ReactiveUI;
using CommandLine;

#pragma warning disable CA1416

namespace ADDSMock
{
    internal class EntryPoint
    {
        private static void Main(string[] args)
        {
            var isInteractive = false;

            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(o =>
            {
                if (o.IsInteractive)
                {
                    isInteractive = true;
                }
            });

            if (isInteractive)
            {
                var thread = new Thread(() =>
                {
                    var builder = AppBuilder.Configure<InteractiveApplication>().UsePlatformDetect().WithInterFont().UseReactiveUI().LogToTrace();
                    builder.StartWithClassicDesktopLifetime(args);
                });

                thread.SetApartmentState(ApartmentState.STA);

                thread.Start();
                thread.Join();
            }
            else
            {
                ConsoleApplication.Run(args);
            }

            //return ExitCodes.Success;
        }
    }
}
