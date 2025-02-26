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
                .WithPath("/v1/permits/s100")
                .UsingPost()
                .WithBody(new RegexMatcher(".*200OKResponse.*"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(mockService.Files.FirstOrDefault()?.Name)
        );
    server
        .Given(
            Request.Create()
                .WithPath("/v1/permits/s100")
                .UsingPost()
                .WithBody(new RegexMatcher(".*400BadRequestResponse.*"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Invalid Product or UPN.")
        );
    server
        .Given(
            Request.Create()
                .WithPath("/v1/permits/s100")
                .UsingPost()
                .WithBody(new RegexMatcher(".*500InternalServerErrorResponse.*"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(500)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Internal server error.")
        );
}
