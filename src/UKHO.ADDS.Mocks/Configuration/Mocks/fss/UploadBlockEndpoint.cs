using UKHO.ADDS.Mocks.Domain;
using UKHO.ADDS.Mocks.Domain.Headers;
using UKHO.ADDS.Mocks.Domain.Markdown;
using UKHO.ADDS.Mocks.Domain.Mocks;
using UKHO.ADDS.Mocks.Domain.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class UploadBlockEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPut("/batch/{batchId}/files/{fileName}/{blockId}", (string batchId, string filename, string blockId, HttpRequest request, HttpResponse response) =>
            {
                EchoHeaders(request, response, [WellKnownHeader.CorrelationId]);
                var state = GetState(request);

                if (request.Body.Length == 0)
                {
                    return Results.BadRequest("Body required");
                }

                var file = AppendFile(filename, request.Body, true);

                if (file.IsFailure(out var error))
                {
                    return Results.BadRequest(error.Message);
                }

                switch (state)
                {
                    case WellKnownState.Default:

                            return Results.Created();

                    case WellKnownState.BadRequest:
                        return Results.Json(new
                        {
                            correlationId = request.Headers[WellKnownHeader.CorrelationId],
                            errors = new[]
                            {
                                    new
                                    {
                                        source = "Upload Block",
                                        description = "Invalid batchId."
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
                    d.Append(new MarkdownHeader("Upload a file block", 3));
                    d.Append(new MarkdownParagraph("Just returns a 201, won't actually upload anything"));
                });
    }
}
