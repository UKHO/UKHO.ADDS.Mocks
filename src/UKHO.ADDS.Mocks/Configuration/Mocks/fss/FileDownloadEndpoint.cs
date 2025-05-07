using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class FileDownloadEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapGet("/batch/{batchId}/files/{fileName}", (string batchId, string fileName, HttpRequest request) =>
            {
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:

                        var pathResult = endpoint.GetFile("readme.txt");

                        if (pathResult.IsSuccess(out var file))
                        {
                            return Results.File(file.Path, MimeType.Text.Plain);
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
                    d.Append(new MarkdownHeader("Download a file", 3));
                    d.Append(new MarkdownParagraph("Downloads readme.txt."));
                });
    }

}
