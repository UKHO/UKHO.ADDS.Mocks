using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using System.Text.RegularExpressions;
using WireMock.Http;
using System.Linq;
using Newtonsoft.Json.Linq;
using WireMock.Types;
using System;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch/(.*)/files/(.*)";

    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithPath(new RegexMatcher(@"/batch/[a-fA-F0-9-]{36}/files/"))
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithCallback(request =>
                {
                    var pathSegments = request.PathSegments;
                    var fileName = pathSegments.ElementAtOrDefault(4);
                    var fileResponse = System.IO.File.ReadAllBytes(mockService.Files.Where(x => x.Name == "MockFile.txt").Select(x => x.Path).FirstOrDefault());

                    return new WireMock.ResponseMessage
                    {
                        StatusCode = 200,
                        Headers = new Dictionary<string, WireMockList<string>>
                        {
                            { "Content-Type", "application/octet-stream" },
                            { "Content-Disposition", $"attachment; filename=\"{fileName}\""}
                        },
                        BodyData = new WireMock.Util.BodyData
                        {
                            BodyAsBytes = fileResponse,
                            DetectedBodyType = BodyType.Bytes
                        }
                    };
                })
        );

    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(@"/batch/[a-fA-F0-9-]{36}/files/"))
                .WithHeader("_X-Correlation-ID", "400-invalidbatchid-guid-fss")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"error\": \"Invalid batchId.\"}")
        );
}
