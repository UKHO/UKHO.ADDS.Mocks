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
                .WithHeader("Last-Modified", "2025-01-01T00:00:00Z")
                .WithBodyFromFile(mockService.Files.FirstOrDefault()?.Path)
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("If-Modified-Since", "2025-01-01T00:00:00Z")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(304)
                .WithHeader("Last-Modified", "2025-01-01T00:00:00Z")
        );

    server
    .Given(
        Request.Create()
            .WithUrl(new RegexMatcher(urlPattern))
            .WithHeader("_X-Correlation-ID", "304-notmodified-guid-scs")
            .UsingGet()
    )
    .RespondWith(
        Response.Create()
            .WithStatusCode(304)            
            .WithHeader("Last-Modified", "2025-01-01T00:00:00Z")            
    );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("If-Modified-Since", "20221027")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Bad request.")
        );   

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-scs")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Bad request.")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "500-internalserver-guid-scs")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(500)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Internal server error.")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "401-unauthorised-guid-scs")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Unauthorised.")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-scs")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Forbidden.")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "404-notfound-guid-scs")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(404)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Not found.")
        );

    server
       .Given(
           Request.Create()
               .WithUrl(new RegexMatcher(urlPattern))
               .WithHeader("_X-Correlation-ID", "415-unsupportedmediatype-guid-scs")
               .UsingGet()
       )
       .RespondWith(
           Response.Create()
               .WithStatusCode(415)
               .WithHeader("Content-Type", "application/json")
               .WithBody("Unsupported Media Type.")
       );
}
