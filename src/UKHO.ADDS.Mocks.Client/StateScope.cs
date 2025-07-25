namespace UKHO.ADDS.Mocks.Client
{
    public class StateScope : IAsyncDisposable
    {
        private readonly MockHttpClientFactory _clientFactory;
        private readonly string _session;

        private bool _disposed;

        public StateScope(MockHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _disposed = false;

            _session = Guid.NewGuid().ToString("N");
        }

        public async Task SetState(string servicePrefix, string endpointName, string state)
        {
            var client = _clientFactory.CreateClient();

            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(StateScope));
            }

            try
            {
                var response = await client.PostAsync($"/states/endpoints/{_session}/{servicePrefix}/{endpointName}/{state}", null);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"SetState failed: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            var client = _clientFactory.CreateClient();

            if (_disposed)
            {
                return;
            }

            _disposed = true;

            try
            {
                var response = await client.DeleteAsync($"/states/endpoints/{_session}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"DisposeAsync failed to reset state: {ex.Message}");
            }
        }
    }
}
