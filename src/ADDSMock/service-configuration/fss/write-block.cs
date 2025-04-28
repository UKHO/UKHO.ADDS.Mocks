using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch/(.*)/files/(.*)";

    // 204 No Content Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPut()
                .WithBody(new JsonMatcher(@"
                {
                    ""blockIds"": [""string""]
                }"))
                .WithBody(new RegexMatcher(@".*"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(204)
                .WithHeader("X-Correlation-ID", "204-No-Content-guid-fss-write-block")
        );

    // 400 Bad Request Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(@"/batch/[a-fA-F0-9-]{36}/files/"))
                .WithHeader("X-Correlation-ID", "400-badrequest-guid-fss-write-block")
                .UsingPut()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("X-Correlation-ID", "400-badrequest-guid-fss-write-block")
                .WithBodyAsJson(new
                {
                    correlationId = "400-badrequest-guid-fss-write-block",
                    errors = new[]
                    {
                        new
                        {
                            source = "Write Block",
                            description = "Invalid batchId."
                        }
                    }
                })
        );
}
