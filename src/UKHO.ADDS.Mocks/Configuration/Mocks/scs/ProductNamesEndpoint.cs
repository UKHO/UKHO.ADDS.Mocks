using UKHO.ADDS.Mocks.Configuration.Mocks.scs.ResponseGenerator;
using UKHO.ADDS.Mocks.Headers;
using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.scs
{
    public class ProductNamesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPost("/v2/products/{productType}/ProductNames", async (string productType, HttpRequest request, HttpResponse response) =>
                {
                    EchoHeaders(request, response, [WellKnownHeader.CorrelationId]);

                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                        {
                            switch (productType.ToLowerInvariant())
                            {
                                case "s100":

                                    response.GetTypedHeaders().LastModified = DateTime.UtcNow;
                                    return await ScsResponseGenerator.ProvideProductNamesResponse(request);

                                default:

                                    return Results.Json(new
                                    {
                                        correlationId = request.Headers[WellKnownHeader.CorrelationId],
                                        errors = new[]
                                        {
                                            new
                                            {
                                                source = "No productType set",
                                                description = "Bad Request."
                                            }
                                        }
                                    }, statusCode: 400);
                            }
                        }

                        case WellKnownState.BadRequest:
                            return Results.Json(new
                            {
                                correlationId = request.Headers[WellKnownHeader.CorrelationId],
                                errors = new[]
                                {
                                    new
                                    {
                                        source = "Product Names",
                                        description = "Bad Request."
                                    }
                                }
                            }, statusCode: 400);

                        case WellKnownState.UnsupportedMediaType:
                            return Results.Json(new
                            {
                                type = "https://example.com",
                                title = "Unsupported Media Type",
                                status = 415,
                                traceId = "00-012-0123-01"
                            }, statusCode: 415);

                        case WellKnownState.InternalServerError:
                            return Results.Json(new
                            {
                                correlationId = request.Headers[WellKnownHeader.CorrelationId],
                                details = "Internal Server Error"
                            }, statusCode: 500);

                        default:
                            // Just send default responses
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Product Names Endpoint", 3));
                    d.Append(new MarkdownParagraph("This endpoint is used to retrieve product names based on the product type."));
                });
    }
}
