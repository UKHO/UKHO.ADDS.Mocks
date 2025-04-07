using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(".*/v2/catalogues/s100/basic.*"))
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(mockService.Files.FirstOrDefault()?.Path)
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(".*/v2/catalogues/s100/basic.*"))
                .WithHeader("If-Modified-Since", "2025-01-01T00:00:00Z")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(304)
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(".*/v2/catalogues/s100/basic.*"))
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
                .WithUrl(new RegexMatcher(".*/v2/catalogues/s100/basic.*"))
                .WithHeader("If-Modified-Since", "3000-01-01T00:00:00Z")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(500)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Internal server error.")
        );
}
