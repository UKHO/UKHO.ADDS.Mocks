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
                .WithBody("Bad request.")
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
                .WithBody("Internal server error.")
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
                .WithBody("Unauthorised.")
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
                .WithBody("Forbidden.")
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
                .WithBody("Not found.")
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
               .WithHeader("_X-Correlation-ID", "415-unsupportedmediatype-guid-scs-basic-catalogue")
               .WithBody("Unsupported Media Type.")
       );
}
