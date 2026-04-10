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

            builder.AddProject<UKHO_ADDS_Mocks_SampleService>(ProcessNames.SampleService)
                .WithDashboard("Dashboard");

            await builder.Build().RunAsync();

            return 0;
        }
    }
}
