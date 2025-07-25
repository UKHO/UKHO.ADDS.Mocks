using System.Net;
using UKHO.ADDS.Mocks.Client;
using UKHO.ADDS.Mocks.States;
using Xunit;

namespace UnitTestExamples
{
    public class StateExamples
    {
        public const string MockUri = "https://localhost:1234/sample/files";

        private readonly MockHttpClientFactory _factory;

        public StateExamples() => _factory = new MockHttpClientFactory();

        public async Task SetMockToDefaultState()
        {
            var client = _factory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, MockUri);

            // No need to set anything, the mock is in the default state 

            var response = await client.SendAsync(request);

            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        public async Task SetStatePerTest()
        {
            // Note - scope is in a using block to ensure it is disposed of after the test, so that the state is reset
            await using var scope = new StateScope(_factory);

            await scope.SetState("sample", "GetFilesEndpoint", WellKnownState.BadRequest);

            var client = _factory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, MockUri);
            var response = await client.SendAsync(request);

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        public async Task SetMockToUnauthorisedState()
        {
            var client = _factory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, MockUri);

            // Set the mock state for this call only
            _factory.SetPerRequestState(WellKnownState.Unauthorized);

            var response = await client.SendAsync(request);

            _factory.ResetState();

            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        public async Task SetMockToCustomState()
        {
            var client = _factory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, MockUri);

            // Set the mock state for this call only
            _factory.SetPerRequestState("get-jpeg");

            var response = await client.SendAsync(request);

            _factory.ResetState();

            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }
    }
}
