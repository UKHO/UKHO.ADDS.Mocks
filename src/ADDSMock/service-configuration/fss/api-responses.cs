using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch/{batchId}/files/{fileName}.*";

    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithPath(new RegexMatcher(@"/batch/[a-fA-F0-9-]{36}/files/"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/octet-stream")
                .WithBodyFromFile(mockService.Files.Where(x => x.Name == "MockFile.txt").Select(x => x.Path).FirstOrDefault())
        );

    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(@"^(?!/fss/batch/[a-fA-F0-9-]{36}/files/).*"))
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"error\": \"Invalid batchId.\"}")
        );
}
