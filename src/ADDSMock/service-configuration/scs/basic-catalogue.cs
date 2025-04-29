using ADDSMock.Domain.Mappings;
using ADDSMock.Constants;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/v2/catalogues/s100/basic.*";
    var EndPoint = "scs-basic-catalogue";

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader("Last-Modified", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.CreatedCorrelationId}{EndPoint}")
                .WithBodyFromFile(mockService.Files.FirstOrDefault()?.Path)
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.NotModifiedCorrelationId}{EndPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(304)
                .WithHeader("Last-Modified", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.NotModifiedCorrelationId}{EndPoint}")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
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
                            source = "Basic Catalogue",
                            description = "Provided date format is not valid."
                        }
                    }
                })
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.InternalServerErrorCorrelationId}{EndPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(500)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.InternalServerErrorCorrelationId}{EndPoint}")
                .WithBodyAsJson(new
                {
                    correlationId = $"{MockConstants.InternalServerErrorCorrelationId}{EndPoint}",
                    details = "Internal Server Error"
                })
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.UnauthorizedCorrelationId}{EndPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.UnauthorizedCorrelationId}{EndPoint}")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{EndPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{EndPoint}")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.FileNotFoundCorrelationId}{EndPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(404)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.FileNotFoundCorrelationId}{EndPoint}")
                .WithBodyAsJson(new
                {
                    correlationId = $"{MockConstants.FileNotFoundCorrelationId}{EndPoint}",
                    details = "Not Found"
                })
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.RangeNotSatisfiableCorrelationId}{EndPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(415)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithBodyAsJson(new
                {
                    type = "https://example.com",
                    title = "Unsupported Media Type",
                    status = 415,
                    traceId = "00-012-0123-01"
                })
        );
}
