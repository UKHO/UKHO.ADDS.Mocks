using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch/(.*)/expiry";

    // 204 No Content Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingPut()
                .WithBody(new RegexMatcher(@"\{\s*""expiryDate"":\s*""\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z""\s*\}"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(204)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "204-nocontent-guid-fss-batch-expiry")
        );

    // 400 Bad Request Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingPut()
                .WithHeader("X-Correlation-ID", "400-badrequest-guid-fss-batch-expiry")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("X-Correlation-ID", "400-badrequest-guid-fss-batch-expiry")
                .WithBodyAsJson(new
                {
                    correlationId = "400-badrequest-guid-fss-batch-expiry",
                    errors = new[]
                    {
                        new
                        {
                            source = "Batch expiry",
                            description = "Batch ID does not exist or invalid date."
                        }
                    }
                })
        );
}
