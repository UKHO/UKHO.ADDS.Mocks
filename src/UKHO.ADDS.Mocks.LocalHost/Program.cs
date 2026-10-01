using Projects;
using Serilog;
using UKHO.ADDS.Mocks.LocalHost.Constants;
using UKHO.ADDS.Mocks.LocalHost.Extensions;

namespace UKHO.ADDS.Mocks.LocalHost
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("ADDS Mock Host Aspire Orchestrator");

            var builder = DistributedApplication.CreateBuilder(args);

#if NET10_0_OR_GREATER
            const string targetFramework = "net10.0";
#else
            const string targetFramework = "net9.0";
#endif

            // Aspire 9 launches projects with dotnet run, which requires a framework for multi-targeted projects.
            builder.AddProject<UKHO_ADDS_Mocks_SampleService>(ProcessNames.SampleService)
                .WithArgs("--framework", targetFramework)
                .WithDashboard("Dashboard");

            await builder.Build().RunAsync();

            return 0;
        }
    }
}
