using UKHO.ADDS.Mocks.Domain;
using UKHO.ADDS.Mocks.Domain.Markdown;
using UKHO.ADDS.Mocks.Domain.Mocks;
using UKHO.ADDS.Mocks.Domain.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fssmsi
{
    public class GetFssMsiBatchAttributesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapGet("/attributes/search", (HttpRequest request) =>
                {
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

                        default:
                            // Just send default responses
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Gets Batch Attributes (MSI)", 3));
                    d.Append(new MarkdownParagraph("This is driven from a static file attributes.json, and simulates a basic attribute query"));
                });
    }
}
