using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace UKHO.ADDS.Mocks.Functional.Tests
{
    internal sealed class SampleServiceFixture
    {
        public Uri BaseAddress { get; private set; } = null!;

        private DistributedApplication _app = null!;

        public async Task StartAsync()
        {
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.UKHO_ADDS_Mocks_LocalHost>();
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });
            _app = await appHost.BuildAsync();

            var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
            await _app.StartAsync();
            await resourceNotificationService.WaitForResourceAsync("adds-mocks-sample", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
            BaseAddress = _app.GetEndpoint("adds-mocks-sample");
        }

        public async Task StopAsync()
        {
            if (_app != null)
            {
                await _app.StopAsync();
                await _app.DisposeAsync();
            }
        }
    }
}
