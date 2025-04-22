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

    // 200 OK Response with File Download
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(@"/batch/[a-fA-F0-9-]{36}/files/"))
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithCallback(request =>
                {
                    var pathSegments = request.PathSegments;
                    var fileName = pathSegments.ElementAtOrDefault(4);
                    var fileResponse = System.IO.File.ReadAllBytes(
                        mockService.Files
                            .Where(x => x.Name == "mock-file.txt")
                            .Select(x => x.Path)
                            .FirstOrDefault()
                    );

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

    // 400 Bad Request Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(@"/batch/[a-fA-F0-9-]{36}/files/"))
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-file-download")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-file-download")
                .WithBodyAsJson(new
                {
                    correlationId = "400-badrequest-guid-fss-file-download",
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

    // 401 Unauthorized Response
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
        );

    // 403 Forbidden Response
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
        );

    // 404 File Not Found Response
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
        );

    // 410 Gone Response
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
        );

    // 416 Range Not Satisfiable Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader("_X-Correlation-ID", "416-rangenotsatifiable-guid-fss-file-download")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(416)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "416-rangenotsatifiable-guid-fss-file-download")
        );

    // 429 Too Many Requests Response
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
        );

    // 307 Temporary Redirect Response
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
        );
}
