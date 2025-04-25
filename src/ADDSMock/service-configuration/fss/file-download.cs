using ADDSMock.Domain.Mappings;
using ADDSMock.Constants;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using System.Text.RegularExpressions;
using WireMock.Http;
using System.Linq;
using WireMock.Types;
using System;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch/(.*)/files/(.*)";
    var EndPoint = "fss-file-download";

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
                                { MockConstants.ContentTypeHeader, "application/octet-stream" },
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
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.BadRequestCorrelationId}{EndPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.BadRequestCorrelationId}{EndPoint}")
                .WithBodyAsJson(new
                {
                    correlationId = $"{MockConstants.BadRequestCorrelationId}{EndPoint}",
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
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.UnauthorizedCorrelationId}{EndPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.UnauthorizedCorrelationId}{EndPoint}")
        );

    // 403 Forbidden Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{EndPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{EndPoint}")
        );

    // 404 File Not Found Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.FileNotFoundCorrelationId}{EndPoint}")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(404)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.FileNotFoundCorrelationId}{EndPoint}")
        );

    // 410 Gone Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.GoneCorrelationId}{EndPoint}")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(410)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.GoneCorrelationId}{EndPoint}")
        );

    // 416 Range Not Satisfiable Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.RangeNotSatisfiableCorrelationId}{EndPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(416)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.RangeNotSatisfiableCorrelationId}{EndPoint}")
        );

    // 429 Too Many Requests Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.TooManyRequestsCorrelationId}{EndPoint}")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(429)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader("Retry-After", "10")
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.TooManyRequestsCorrelationId}{EndPoint}")
        );

    // 307 Temporary Redirect Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingGet()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.TemporaryRedirectCorrelationId}{EndPoint}")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(307)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader("Redirect-Location", "https://example.com/redirect")
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.TemporaryRedirectCorrelationId}{EndPoint}")
        );
}
