using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/v2/catalogues/s100/basic.*";

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Last-Modified", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .WithHeader("_X-Correlation-ID", "200-ok-guid-scs-basic-catalogue")
                .WithBodyFromFile(mockService.Files.FirstOrDefault()?.Path)
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "304-notmodified-guid-scs-basic-catalogue")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(304)
                .WithHeader("Last-Modified", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .WithHeader("_X-Correlation-ID", "304-notmodified-guid-scs-basic-catalogue")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-scs-basic-catalogue")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-scs-basic-catalogue")
                .WithBodyAsJson(new
                {
                    correlationId = "400-badrequest-guid-scs-basic-catalogue",
                    errors = new[]
                    {
                        new
                        {
                            source = "Basic Catalogue",
                            description = "Provided date format is not valid."
                        }
                    }
                })
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "500-internalserver-guid-scs-basic-catalogue")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(500)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "500-internalserver-guid-scs-basic-catalogue")
                .WithBodyAsJson(new
                {
                    correlationId = "500-internalserver-guid-scs-basic-catalogue",
                    details = "Internal Server Error"
                })
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "401-unauthorised-guid-scs-basic-catalogue")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "401-unauthorised-guid-scs-basic-catalogue")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-scs-basic-catalogue")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-scs-basic-catalogue")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "404-notfound-guid-scs-basic-catalogue")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(404)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "404-notfound-guid-scs-basic-catalogue")
                .WithBodyAsJson(new
                {
                    correlationId = "404-notfound-guid-scs-basic-catalogue",
                    details = "Not Found"
                })
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "415-unsupportedmediatype-guid-scs-basic-catalogue")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(415)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new
                {
                    type = "https://example.com",
                    title = "Unsupported Media Type",
                    status = 415,
                    traceId = "00-012-0123-01"
                })
        );
}
