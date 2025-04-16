using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;


public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = "/batch/{batchId}/files/{fileName}";

    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithPath(new RegexMatcher(@"/batch/[a-zA-Z0-9-]+/files/[a-zA-Z0-9]+\.[0-9]{3}$")) // Filter for valid file paths
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/octet-stream")
        );

    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingGet()
                .WithPath(new RegexMatcher(@"^(?!/fss/batch/s100-batch/files/[a-zA-Z0-9]+\.[0-9]{3}$).*")) // Filter for invalid file
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithBody("invalid batch id or filename.")
        );
}
