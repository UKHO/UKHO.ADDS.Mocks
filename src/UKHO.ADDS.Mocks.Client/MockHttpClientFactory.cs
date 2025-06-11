namespace UKHO.ADDS.Mocks.Client
{
    public class MockHttpClientFactory : IHttpClientFactory
    {
        private const string StateHeader = "x-addsmockstate";
        private const string DefaultState = "default";

        private string _state;

        public MockHttpClientFactory() => _state = DefaultState;

        public HttpClient CreateClient(string name) => new(new HeaderInjectingHandler(StateHeader, () => _state));

        public void SetPerRequestState(string state) => _state = state;

        public void ResetState() => _state = DefaultState;
    }
}
