using System.Net;
using UKHO.ADDS.Mocks.Client;
using Xunit;

namespace UnitTestExamples
{
    public class StateExamples
    {
        public const string MockUri = "https://localhost:1234/sample/files";

        private readonly MockHttpClientFactory _factory;

        public StateExamples()
        {
            _factory = new MockHttpClientFactory();
        }

        [Fact]
        public async Task SetMockToDefaultState()
        {
            var client = _factory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, MockUri);

            // No need to set anything, the mock is in the default state 

            var response = await client.SendAsync(request);

            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task SetMockToUnauthorisedState()
        {
            var client = _factory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, MockUri);

            // Set the mock state for this call only
            _factory.SetState("unauthorised");

            var response = await client.SendAsync(request);

            _factory.ResetState();

            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }
    }
}
