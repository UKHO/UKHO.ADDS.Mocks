using ADDSMock.Domain.Mappings;
using ADDSMock.Constants;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = "/batch/(.*)/files/(.*)";
    var endPoint = "fss-add-file";

    // 201 Created Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader("X-Content-Size", new RegexMatcher(".*"))
                .WithBody(new JsonMatcher(@"
                   {
                    ""attributes"": [
                      {
                        ""key"": ""string"",
                        ""value"": ""string""
                      } ]
                   }"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(201)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.CreatedCorrelationId}{endPoint}")
        );

    // 400 Bad Request Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.BadRequestCorrelationId}{endPoint}")
                .UsingPost()
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
                                source = "Add File",
                                description = "Batch ID is missing in the URI."
                            }
                    }
                })
        );

    // 401 Unauthorized Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.UnauthorizedCorrelationId}{endPoint}")
                .UsingPost()
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
                .WithPath(urlPattern)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{endPoint}")
                .UsingPost()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{endPoint}")
        );

    // 429 Too Many Requests Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
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
