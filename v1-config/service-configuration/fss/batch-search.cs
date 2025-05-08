using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using System.Text.RegularExpressions;
using WireMock.Http;
using System.Linq;
using ADDSMock.ResponseGenerator;
using ADDSMock.Constants;
using System;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch.*";
    var endPoint = "fss-batch-search";

    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .WithParam("limit", new RegexMatcher(".*"))
                .WithParam("start", new RegexMatcher(".*"))
                .WithParam("$filter", "*")
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithCallback(request =>
                {
                    var templatePath = mockService.Files
                        .Where(x => x.Name == "search-product.json")
                        .Select(x => x.Path)
                        .FirstOrDefault();
                    return FSSResponseGenerator.ProvideSearchFilterResponse(request, templatePath);
                })
        );

    server
         .Given(
             Request.Create()
                .WithPath(urlPattern)
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
                .WithPath(urlPattern)
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
                .WithPath(urlPattern)
                .UsingGet()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.TooManyRequestsCorrelationId}{endPoint}")
         )
         .RespondWith(
             Response.Create()
                .WithStatusCode(429)
                .WithHeader(MockConstants.ContentTypeHeader, MockConstants.ApplicationJson)
                .WithHeader("Retry-After", "10")
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.TooManyRequestsCorrelationId}{endPoint}")
         );

    server
         .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingGet()
                .WithHeader(MockConstants.CorrelationIdHeader, $"{MockConstants.BadRequestCorrelationId}{endPoint}")
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
                            new { source = "Search Product", description = "Bad Request" }
                    }
                })
         );
}
