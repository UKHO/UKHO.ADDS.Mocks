using ADDSMock.Domain.Services;
using ADDSMock.Domain.Services.Runtime;
using ADDSMock.Extensions;
using Serilog;

namespace ADDSMock.Applications.Console
{
    internal class ConsoleApplication
    {
        public static void Run(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).UseSerilog(Log.Logger)
                .ConfigureLogging(logging => logging.AddConsole()).ConfigureServices((host, services) => ConfigureServices(services, host.Configuration));
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMappingService, MappingService>();
            services.AddConsole();
        }
    }
}
