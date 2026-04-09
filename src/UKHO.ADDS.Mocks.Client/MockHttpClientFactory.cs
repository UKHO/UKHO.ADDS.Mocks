namespace UKHO.ADDS.Mocks.Client
{
    public sealed class MockHttpClientFactory : IHttpClientFactory, IDisposable
    {
        private const string StateHeader = "x-addsmockstate";
        private const string DefaultState = "default";

        private readonly AsyncLocal<string?> _state;
        private readonly HttpMessageHandler _innerHandler;

        private bool _disposed;

        public MockHttpClientFactory()
        {
            _state = new AsyncLocal<string?>();
            _innerHandler = new HttpClientHandler();
        }

        public HttpClient CreateClient(string name = "")
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var handler = new HeaderInjectingHandler(StateHeader, GetCurrentState) { InnerHandler = _innerHandler };

            return new HttpClient(handler, false);
        }

        public void SetPerRequestState(string state)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(state);
            _state.Value = state;
        }

        public void ResetState() => _state.Value = null;

        private string GetCurrentState() => _state.Value ?? DefaultState;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _innerHandler.Dispose();
            _disposed = true;
        }
    }
}
