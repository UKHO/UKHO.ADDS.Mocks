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
        )

    .RespondWith(
        Response.Create()
            .WithCallback(request =>
            {
                return FSSResponseProvider.ProvideSearchFilterResponse(request);
            })
    );
}




