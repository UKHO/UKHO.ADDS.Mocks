using ADDSMock.Domain.Mappings;
using ADDSMock.Constants;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch/(.*)/files/(.*)/(.*)";
    var endPoint = "fss-upload-block";

    // 201 Created Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingPut()
                .WithHeader("Content-Lengths", new RegexMatcher(".*"))
                .WithHeader("Content-MD5", new RegexMatcher(".*"))
                .WithHeader(MockConstants.ContentTypeHeader, new RegexMatcher(".*"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(201)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.CreatedCorrelationId}{endPoint}")
        );

    // 400 Bad Request Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.BadRequestCorrelationId}{endPoint}")
                .UsingPut()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.BadRequestCorrelationId}{endPoint}")
                .WithBodyAsJson(new
                {
                    correlationId = $"{MockConstants.BadRequestCorrelationId}{endPoint}",
                    errors = new[]
                    {
                        new
                        {
                            source = "Upload Block",
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
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.UnauthorizedCorrelationId}{endPoint}")
                .UsingPut()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.UnauthorizedCorrelationId}{endPoint}")
        );

    // 403 Forbidden Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{endPoint}")
                .UsingPut()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{endPoint}")
        );

    // 413 Payload Too Large Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.PayloadTooLargeCorrelationId}{endPoint}")
                .UsingPut()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(413)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.PayloadTooLargeCorrelationId}{endPoint}")
        );

    // 429 Too Many Requests Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(urlPattern))
                .UsingPut()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.TooManyRequestsCorrelationId}{endPoint}")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(429)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.TooManyRequestsCorrelationId}{endPoint}")
                .WithHeader("Retry-After", "10")
        );
}
