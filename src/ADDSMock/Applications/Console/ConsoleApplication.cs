using ADDSMock.Domain.Services;
using ADDSMock.Extensions;
using Serilog;

namespace ADDSMock.Applications.Console
{
    internal class ConsoleApplication
    {
        public async Task RunAsync(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var consoleResult = builder.Services.AddConsole();

            if (consoleResult.IsFailed)
            {
                Log.Fatal(consoleResult.Error.Message);
                return;
            }

            builder.Host.UseSerilog(Log.Logger);
            builder.Services.AddAuthorization();

            var application = builder.Build();

            application.UseHttpsRedirection();
            application.UseAuthorization();

            var wireMockService = application.Services.GetRequiredService<IWireMockService>();
            var wireMockStartResult = wireMockService.Start();

            if (wireMockStartResult.IsFailed)
            {
                Log.Fatal(wireMockStartResult.Error.Message);
                return;
            }

            var mappingService = application.Services.GetRequiredService<IMappingService>();
            var readMappingsResult = await mappingService.ReadMappingsAsync();

            if (readMappingsResult.IsFailed)
            {
                Log.Fatal(readMappingsResult.Error.Message);
                return;
            }

            await mappingService.ExecuteMappingsAsync(wireMockService);

            await application.RunAsync();

            wireMockService.Stop();
        }
    }
}
