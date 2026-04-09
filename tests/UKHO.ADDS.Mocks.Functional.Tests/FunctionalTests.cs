using System.Net;

namespace UKHO.ADDS.Mocks.Functional.Tests
{
    public class FunctionalTests
    {
        private SampleServiceFixture _fixture = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _fixture = new SampleServiceFixture();
            await _fixture.StartAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _fixture.StopAsync();
        }

        [Test]
        public async Task GetFiles_Returns_Default_Response()
        {
            var client = _fixture.Factory.CreateClient();
            var uri = new Uri(_fixture.BaseAddress, "/sample/files");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            using var response = await client.SendAsync(request);
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
            var client = _fixture.Factory.CreateClient();
            var uri = new Uri(_fixture.BaseAddress, "/sample/files");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("x-addsmockstate", "get-jpeg");

            using var response = await client.SendAsync(request);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("image/jpeg"));
            }
        }
    }
}
