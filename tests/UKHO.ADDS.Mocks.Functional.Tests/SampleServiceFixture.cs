using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using UKHO.ADDS.Mocks.LocalHost.Constants;

namespace UKHO.ADDS.Mocks.Functional.Tests
{
    internal sealed class SampleServiceFixture
    {
        public Uri BaseAddress { get; private set; } = null!;

        private DistributedApplication _app = null!;

        public async Task StartAsync()
        {
            using var startupTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            var cancellationToken = startupTimeout.Token;
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.UKHO_ADDS_Mocks_LocalHost>(cancellationToken);
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });
            _app = await appHost.BuildAsync(cancellationToken);

            var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
            await _app.StartAsync(cancellationToken);
            await resourceNotificationService.WaitForResourceAsync(ProcessNames.SampleService, KnownResourceStates.Running, cancellationToken);
            BaseAddress = _app.GetEndpoint(ProcessNames.SampleService);
        }

        public async Task StopAsync()
        {
            if (_app != null)
            {
                using var shutdownTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                try
                {
                    await _app.StopAsync(shutdownTimeout.Token);
                }
                finally
                {
                    await _app.DisposeAsync();
                }
            }
        }
    }
}
