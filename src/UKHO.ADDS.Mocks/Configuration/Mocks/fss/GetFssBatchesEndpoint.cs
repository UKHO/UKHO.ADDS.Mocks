using System.Text.RegularExpressions;
using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class GetFssBatchesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapGet("/batch", (HttpRequest request) =>
            {
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:

                        var pathResult = endpoint.GetFile("batchsearchresult.json");

                        if (pathResult.IsSuccess(out var file))
                        {
                            return Results.File(file.Path, contentType: MimeType.Application.Json);
                        }

                        return Results.NotFound("Could not find the path in the /files GET method");

                    default:
                        // Just send default responses
                        return WellKnownStateHandler.HandleWellKnownState(state);
                }
            })
            .Produces<string>()
            .WithEndpointMetadata(endpoint, d =>
            {
                d.Append(new MarkdownHeader("Gets Batches", 3));
                d.Append(new MarkdownParagraph("This is driven from a static file batchsearchresult.json"));
            });
        }
    }
}
