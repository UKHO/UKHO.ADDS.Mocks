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
                .WithHeader("_X-Correlation-ID", "400-invalidbatchid-guid-fss-file-downloads")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"error\": \"Invalid batchId.\"}")
        );

    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "401-unauthorized-fss-file-downloads")
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
                .WithHeader("_X-Correlation-ID", "403-forbidden-fss-file-downloads")
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
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithHeader("_X-Correlation-ID", "404-filenotfound-fss-file-downloads")
        )
        .RespondWith(
              Response.Create()
                .WithStatusCode(404)
                .WithHeader("Content-Type", "application/json")
                .WithBody("File Not Found")
        );

    server
        .Given(
             Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithHeader("_X-Correlation-ID", "410-gone-fss-file-downloads")
        )
        .RespondWith(
             Response.Create()
                .WithStatusCode(410)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Gone")
        );

    server
        .Given(
             Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader("Range", "416-rangenotsatifiable-fss-file-downloads")
                .UsingGet()
        )
        .RespondWith(
             Response.Create()
                .WithStatusCode(416)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Range Not Satisfiable")
        );

    server
        .Given(
             Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-file-downloads")
        )
        .RespondWith(
             Response.Create()
                .WithStatusCode(429)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Retry-After", "10")
                .WithBody("Too Many Requests")
        );

    server
          .Given(
              Request.Create()
                 .WithPath(new RegexMatcher(urlPattern))
                 .UsingGet()
                 .WithHeader("_X-Correlation-ID", "307-temporaryredirect-fss-file-downloads")
          )
          .RespondWith(
              Response.Create()
                 .WithStatusCode(307)
                 .WithHeader("Content-Type", "application/json")
                 .WithHeader("Redirect-Location", "https://example.com/redirect")
                 .WithBody("Temporary Redirect")
          );
}
