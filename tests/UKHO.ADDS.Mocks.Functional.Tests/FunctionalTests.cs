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

        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string path, string? state = null, HttpContent? content = null)
        {
            if (state is not null)
            {
                _factory.SetPerRequestState(state);
            }

            using var client = _factory.CreateClient();
            var uri = new Uri(_fixture.BaseAddress, path);
            using var request = new HttpRequestMessage(method, uri) { Content = content };
            using var requestTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            try
            {
                return await client.SendAsync(request, requestTimeout.Token);
            }
            finally
            {
                if (state is not null)
                {
                    _factory.ResetState();
                }
            }
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

        [Test]
        public async Task CopyFilesAppend_Returns_Original_Jpeg_Content()
        {
            using var original = await SendRequestAsync(HttpMethod.Get, "/sample/files", "get-jpeg");
            using var copy = await SendRequestAsync(HttpMethod.Post, "/sample/files/copy/append", "get-jpeg");
            var originalBytes = await original.Content.ReadAsByteArrayAsync();
            var copyBytes = await copy.Content.ReadAsByteArrayAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(original.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(copy.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(copy.Content.Headers.ContentType?.MediaType, Is.EqualTo("image/jpeg"));
                Assert.That(originalBytes, Is.Not.Empty);
                Assert.That(copyBytes, Is.EqualTo(originalBytes));
            }
        }

        [Test]
        public async Task CreateFiles_Accepts_Request_Body()
        {
            using var response = await SendRequestAsync(HttpMethod.Post, "/sample/files", content: new ByteArrayContent(new byte[] { 1, 2, 3, 4 }));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
