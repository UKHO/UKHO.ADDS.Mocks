using ADDSMock.Domain.Mappings;
using ADDSMock.Constants;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/v2/catalogues/s100/basic.*";
    var endPoint = "scs-basic-catalogue";

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
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.CreatedCorrelationId}{endPoint}")
                .WithBodyFromFile(mockService.Files.FirstOrDefault()?.Path)
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.NotModifiedCorrelationId}{endPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(304)
                .WithHeader("Last-Modified", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.NotModifiedCorrelationId}{endPoint}")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.BadRequestCorrelationId}{endPoint}")
                .UsingGet()
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
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.InternalServerErrorCorrelationId}{endPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(500)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.InternalServerErrorCorrelationId}{endPoint}")
                .WithBodyAsJson(new
                {
                    correlationId = $"{MockConstants.InternalServerErrorCorrelationId}{endPoint}",
                    details = "Internal Server Error"
                })
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.UnauthorizedCorrelationId}{endPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.UnauthorizedCorrelationId}{endPoint}")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{endPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{endPoint}")
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.FileNotFoundCorrelationId}{endPoint}")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(404)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.FileNotFoundCorrelationId}{endPoint}")
                .WithBodyAsJson(new
                {
                    correlationId = $"{MockConstants.FileNotFoundCorrelationId}{endPoint}",
                    details = "Not Found"
                })
        );

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(urlPattern))
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.RangeNotSatisfiableCorrelationId}{endPoint}")
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
