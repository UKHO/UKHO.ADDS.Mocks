using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.sample
{
    public class CreateFilesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapPost("/files", (HttpRequest request) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                            // ADDS Mock will have the 'default' state unless we have told it otherwise
                            return Results.Ok("File was created ok (it wasn't really)");

                        default:
                            // Just send default responses
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Creates a file", 3));
                    d.Append(new MarkdownParagraph("Just a demo method, won't actually create anything"));
                });
        }
    }
}
