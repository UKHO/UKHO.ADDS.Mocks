using System.Net;
using UKHO.ADDS.Mocks.Client;

namespace UKHO.ADDS.Mocks.Functional.Tests
{
    public class FunctionalTests
    {
        private SampleServiceFixture _fixture = null!;
        private MockHttpClientFactory _factory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _fixture = new SampleServiceFixture();
            _factory = new MockHttpClientFactory();
            await _fixture.StartAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _fixture.StopAsync();
            _factory.Dispose();
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string path, string? state = null)
        {
            if (state is not null)
            {
                _factory.SetPerRequestState(state);
            }

            var client = _factory.CreateClient();
            var uri = new Uri(_fixture.BaseAddress, path);
            var request = new HttpRequestMessage(method, uri);

            var response = await client.SendAsync(request);

            if (state is not null)
            {
                _factory.ResetState();
            }

            return response;
        }

        [Test]
        public async Task GetFiles_Returns_Default_Response()
        {
            using var response = await SendRequestAsync(HttpMethod.Get, "/sample/files");
            var body = await response.Content.ReadAsStringAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(body, Does.Contain("This is a result"));
            }
        }

        [Test]
        public async Task GetFiles_With_PerRequest_State_Returns_Jpeg()
        {
            using var response = await SendRequestAsync(HttpMethod.Get, "/sample/files", "get-jpeg");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("image/jpeg"));
            }
        }
    }
}
