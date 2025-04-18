using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using System.Text.RegularExpressions;
using WireMock.Http;
using System.Linq;
using ADDSMock.Provider;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch.*";
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .WithParam("limit", new RegexMatcher(".*"))
                .WithParam("start", new RegexMatcher(".*"))
                .WithParam("$filter", "*")
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithCallback(request =>
                {
                    return FSSResponseProvider.ProvideSearchFilterResponse(request);
                })
        );

    server
         .Given(
             Request.Create()
                 .WithPath(new RegexMatcher(urlPattern))
                 .WithHeader("_X-Correlation-ID", "400-badrequest-fss-batch-search")
                 .UsingGet()
         )
         .RespondWith(
             Response.Create()
                 .WithStatusCode(400)
                 .WithHeader("Content-Type", "application/json")
                 .WithBody("Bad Request")
         );

    server
         .Given(
             Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "401-unauthorized-batch-search")
                .UsingGet()
         )

         .RespondWith(
             Response.Create()
                .WithStatusCode(401)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Unauthorized")
         );

    server
         .Given(
             Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "403-forbidden-fss-batch-search")
                .UsingGet()
         )
         .RespondWith(
             Response.Create()
                .WithStatusCode(403)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Forbidden")
         );

    server
         .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingGet()
                .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-batch-search")
         )

         .RespondWith(
             Response.Create()
                .WithStatusCode(429)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Retry-After", "10")
                .WithBody("Too Many Requests")
         );
}




