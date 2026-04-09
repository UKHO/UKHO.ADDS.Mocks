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
            using var response = await _fixture.Client.GetAsync("/sample/files");
            var body = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body, Does.Contain("This is a result"));
        }

        [Test]
        public async Task GetFiles_With_PerRequest_State_Returns_Jpeg()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/sample/files");
            request.Headers.Add("x-addsmockstate", "get-jpeg");

            using var response = await _fixture.Client.SendAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("image/jpeg"));
        }
    }
}
