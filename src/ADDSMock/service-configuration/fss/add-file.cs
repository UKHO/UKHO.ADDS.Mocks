using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = "/batch/(.*)/files/(.*)";

    // 201 Created Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader("X-Content-Size", new RegexMatcher(".*"))
                .WithBody(new JsonMatcher(@"
               {
                ""attributes"": [
                  {
                    ""key"": ""string"",
                    ""value"": ""string""
                  }]
               }"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "201-created-guid-fss-add-file")
        );

    // 400 Bad Request Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-add-file")
                .UsingPost()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-add-file")
                .WithBodyAsJson(new
                {
                    correlationId = "400-badrequest-guid-fss-add-file",
                    errors = new[]
                    {
                        new
                        {
                            source = "Add File",
                            description = "Batch ID is missing in the URI."
                        }
                    }
                })
        );

    // 401 Unauthorized Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .WithHeader("_X-Correlation-ID", "401-unauthorized-guid-fss-add-file")
                .UsingPost()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "401-unauthorized-guid-fss-add-file")
        );

    // 403 Forbidden Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-fss-add-file")
                .UsingPost()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-fss-add-file")
        );

    // 429 Too Many Requests Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-add-file")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(429)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-add-file")
                .WithHeader("Retry-After", "10")
        );
}
