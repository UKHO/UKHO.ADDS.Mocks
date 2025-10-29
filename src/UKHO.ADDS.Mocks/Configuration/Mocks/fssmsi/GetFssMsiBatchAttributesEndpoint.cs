using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

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

                            try
                            {
                                var fs = GetFileSystem();
                                var s = fs.OpenFile("/attributes.json", FileMode.Open, FileAccess.Read);
                                return Results.File(s, MimeType.Application.Json);
                            }
                            catch (Exception)
                            {
                                return Results.NotFound("Could not find the path in the /files GET method");
                            }


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
