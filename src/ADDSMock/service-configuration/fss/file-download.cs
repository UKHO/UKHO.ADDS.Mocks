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
                    var fileResponse = System.IO.File.ReadAllBytes(mockService.Files.Where(x => x.Name == "mock-file.txt").Select(x => x.Path).FirstOrDefault());

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
                .WithHeader("_X-Correlation-ID", "400-invalidbatchid-guid-fss-file-download")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "400-invalidbatchid-guid-fss-file-download")
                .WithBody("{\"error\": \"Invalid batchId.\"}")
        );

    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "401-unauthorized-guid-fss-file-download")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "401-unauthorized-guid-fss-file-download")
                .WithBody("Unauthorized")
        );

    server
        .Given(
             Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-fss-file-download")
                .UsingGet()
        )
        .RespondWith(
             Response.Create()
                .WithStatusCode(403)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-fss-file-download")
                .WithBody("Forbidden")
        );

    server
        .Given(
             Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithHeader("_X-Correlation-ID", "404-filenotfound-guid-fss-file-download")
        )
        .RespondWith(
              Response.Create()
                .WithStatusCode(404)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "404-filenotfound-guid-fss-file-download")
                .WithBody("File Not Found")
        );

    server
        .Given(
             Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithHeader("_X-Correlation-ID", "410-gone-guid-fss-file-download")
        )
        .RespondWith(
             Response.Create()
                .WithStatusCode(410)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "410-gone-guid-fss-file-download")
                .WithBody("Gone")
        );

    server
        .Given(
             Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader("Range", "416-rangenotsatifiable-guid-fss-file-download")
                .UsingGet()
        )
        .RespondWith(
             Response.Create()
                .WithStatusCode(416)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Range", "416-rangenotsatifiable-guid-fss-file-download")
                .WithBody("Range Not Satisfiable")
        );

    server
        .Given(
             Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-file-download")
        )
        .RespondWith(
             Response.Create()
                .WithStatusCode(429)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Retry-After", "10")
                .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-file-download")
                .WithBody("Too Many Requests")
        );

    server
          .Given(
              Request.Create()
                 .WithPath(new RegexMatcher(urlPattern))
                 .UsingGet()
                 .WithHeader("_X-Correlation-ID", "307-temporaryredirect-guid-fss-file-download")
          )
          .RespondWith(
              Response.Create()
                 .WithStatusCode(307)
                 .WithHeader("Content-Type", "application/json")
                 .WithHeader("Redirect-Location", "https://example.com/redirect")
                 .WithHeader("_X-Correlation-ID", "307-temporaryredirect-guid-fss-file-download")
                 .WithBody("Temporary Redirect")
          );
}
