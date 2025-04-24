using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch/(.*)/expiry";

    // 410 Gone Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingPut()
                .WithHeader("_X-Correlation-ID", "410-gone-guid-fss-batch-expiry")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(410)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "410-gone-guid-fss-batch-expiry")
        );

    // 204 No Content Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPut()
                .WithBody(new JsonMatcher(@"
                     {""expiryDate"": ""2025-04-09T14:45:33.366Z""}"))
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
                .WithPath(urlPattern)
                .UsingPut()
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-batch-expiry")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-batch-expiry")
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

    // 401 Unauthorized Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPut()
                .WithHeader("_X-Correlation-ID", "401-unauthorised-guid-fss-batch-expiry")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "401-unauthorised-guid-fss-batch-expiry")
        );

    // 403 Forbidden Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPut()
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-fss-batch-expiry")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-fss-batch-expiry")
        );

    // 429 Too Many Requests Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPut()
                .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-batch-expiry")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(429)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-batch-expiry")
                .WithHeader("Retry-After", "10")
        );
}
