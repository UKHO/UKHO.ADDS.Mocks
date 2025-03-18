using System.Net;
using LightResults;
using Serilog;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace ADDSMock.Domain.Services.Runtime
{
    internal class WireMockService : IWireMockService
    {
        private readonly IEnvironmentService _environmentService;
        private readonly ILoggingService _loggingService;
        private string _baseUrl;
        private WireMockServer? _wireMockServer;

        public WireMockService(IEnvironmentService environmentService, ILoggingService loggingService)
        {
            _environmentService = environmentService;
            _loggingService = loggingService;
            _wireMockServer = null;
            _baseUrl = string.Empty;
        }

        public string BaseUrl => _baseUrl;

        public Result Start()
        {
            var configuration = _environmentService.Mock;
            

            var wireMockConfiguration = new WireMockServerSettings
            {
                Port = configuration.Port,
                UseSSL = configuration.UseSsl,
                AllowPartialMapping = true,
                StartAdminInterface = true,
                UseHttp2 = configuration.UseHttp2,
                Logger = _loggingService,
                WatchStaticMappings = false,
                WatchStaticMappingsInSubdirectories = false,
                //Urls = new[] { _baseUrl }
            };

            // TODO Implement file system so that we can store JSON mappings correctly

            try
            {
                _wireMockServer = WireMockServer.Start(wireMockConfiguration);

                _wireMockServer
                    .Given(
                        Request.Create().WithPath(new ExactMatcher("unique/health/thing")).UsingGet()
                    )
                    .RespondWith(
                        Response.Create()
                            .WithStatusCode(HttpStatusCode.OK)
                            .WithHeader("Content-Type", "text/plain")
                            .WithBody("Healthy (override...)!"));

                var scheme = configuration.UseSsl ? "https" : "http";
                _baseUrl = $"{_wireMockServer.Url}/";

                foreach (var service in _environmentService.Services.Configurations)
                {
                    Log.Information($"{service.Name} [{service.Prefix}] started at [{_wireMockServer.Url}/{service.Prefix}/]");
                }

                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail(e.Message);
            }
        }

        public void Stop()
        {
            _wireMockServer?.Stop();
            Log.Information("Service mocks stopped");
        }

        public WireMockServer? WireMockServer => _wireMockServer;
    }
}
