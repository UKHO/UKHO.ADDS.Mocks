using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch/{batchId}/files/{filename}/{blockId}";

    // 201 Created Response with File Download
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPut()
                .WithHeader("Content-Lengths", new RegexMatcher(".*"))
                .WithHeader("Content-MD5", new RegexMatcher(".*"))
                .WithHeader("Content-Type", new RegexMatcher(".*"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(201)
        );

    // 400 Bad Request Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-upload-block")
                .UsingPut()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-upload-block")
                .WithBodyAsJson(new
                {
                    correlationId = "400-badrequest-guid-fss-upload-block",
                    errors = new[]
                    {
                        new
                        {
                            source = "File Download",
                            description = "Invalid batchId."
                        }
                    }
                })
        );
}
