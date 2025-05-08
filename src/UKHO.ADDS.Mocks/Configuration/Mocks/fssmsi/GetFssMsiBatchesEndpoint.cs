using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fssmsi
{
    public class GetFssMsiBatchesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapGet("/batch", (HttpRequest request) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:

                            var pathResult = endpoint.GetFile("annualfiles.json");

                            if (pathResult.IsSuccess(out var file))
                            {
                                return Results.File(file.Path, MimeType.Application.Json);
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
                    d.Append(new MarkdownHeader("Gets Batches (MSI)", 3));
                    d.Append(new MarkdownParagraph("This is driven from a static file annualfiles.json"));
                });
    }
}
