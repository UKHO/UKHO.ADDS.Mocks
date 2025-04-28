using ADDSMock.Domain.Mappings;
using ADDSMock.Constants;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch";
    var EndPoint = "fss-create-batch";

    // 201 Created Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithBody(new JsonMatcher(@"
                    {
                      ""businessUnit"": ""string"",
                      ""acl"": {
                        ""readUsers"": [""string""],
                        ""readGroups"": [""public""]
                      },
                      ""attributes"": [
                        {
                          ""key"": ""string"",
                          ""value"": ""string""
                        }
                      ],
                      ""expiryDate"": ""2025-02-10T11:25:04.982Z""
                    }"))
                .WithBody(new RegexMatcher(@".*"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(201)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.CreatedCorrelationId}{EndPoint}")
                .WithBodyAsJson(new
                {
                    batchId = Guid.NewGuid().ToString()
                })
        );

    // 400 Bad Request Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.BadRequestCorrelationId}{EndPoint}")
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
                            source = "Create Batch",
                            description = "Invalid Expiry Date Format."
                        }
                    }
                })
        );

    // 401 Unauthorized Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.UnauthorizedCorrelationId}{EndPoint}")
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
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{EndPoint}")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.ForbiddenCorrelationId}{EndPoint}")
        );

    // 429 Too Many Requests Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.TooManyRequestsCorrelationId}{EndPoint}")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(429)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.TooManyRequestsCorrelationId}{EndPoint}")
                .WithHeader("Retry-After", "10")
        );
}
