namespace UKHO.ADDS.Mocks.Client
{
    public class MockHttpClientFactory : IHttpClientFactory
    {
        const string StateHeader = "x-addsmockstate";
        const string DefaultState = "default";

        private string _state;

        public MockHttpClientFactory()
        {
            _state = DefaultState;
        }

        public HttpClient CreateClient(string name) => new HttpClient(new HeaderInjectingHandler(StateHeader, () => _state));

        public void SetState(string state)
        {
            _state = state;
        }

        public void ResetState()
        {
            _state = DefaultState;
        }
    }
}
