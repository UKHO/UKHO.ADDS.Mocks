﻿using UKHO.ADDS.Mocks.Headers;
using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class GetFssBatchAttributesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapGet("/attributes/search", (HttpRequest request, HttpResponse response) =>
                {
                    EchoHeaders(request, response, [WellKnownHeader.CorrelationId]);
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:

                            var pathResult = GetFile("attributes.json");

                            if (pathResult.IsSuccess(out var file))
                            {
                                return Results.File(file.Open(), file.MimeType);
                            }

                            return Results.NotFound("Could not find the path in the /files GET method");

                        case WellKnownState.BadRequest:
                            return Results.Json(new
                            {
                                correlationId = request.Headers[WellKnownHeader.CorrelationId],
                                errors = new[]
                                {
                                    new
                                    {
                                        source = "Batch Attribute search",
                                        description = "Bad Request."
                                    }
                                }
                            }, statusCode: 400);

                        case WellKnownState.NotFound:
                            return Results.Json(new
                            {
                                correlationId = request.Headers[WellKnownHeader.CorrelationId],
                                details = "Not Found"
                            }, statusCode: 404);

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
                    d.Append(new MarkdownHeader("Gets Batch Attributes", 3));
                    d.Append(new MarkdownParagraph("This is driven from a static file attributes.json"));
                });
    }
}
