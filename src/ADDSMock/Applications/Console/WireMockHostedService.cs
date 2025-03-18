using ADDSMock.Domain.Services;
using Serilog;

namespace ADDSMock.Applications.Console
{
    public class WireMockHostedService : IHostedService
    {

        private readonly IWireMockService _wireMockService;
        private readonly IMappingService _mappingService;

        public WireMockHostedService(IWireMockService wireMockService, IMappingService mappingService)
        {
            _wireMockService = wireMockService;
            _mappingService = mappingService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var result = _wireMockService.Start();
            if (result.IsFailed)
            {
                throw new Exception(result.Error.Message);
            }
            _ = SetupMock();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _wireMockService.Stop();
            return Task.CompletedTask;
        }

        private async Task SetupMock()
        {
            var readMappingsResult = await _mappingService.ReadMappingsAsync();

            if (readMappingsResult.IsFailed)
            {
                Log.Fatal(readMappingsResult.Error.Message);
                return;
            }
            await _mappingService.ExecuteMappingsAsync(_wireMockService);
        }
    }
}
