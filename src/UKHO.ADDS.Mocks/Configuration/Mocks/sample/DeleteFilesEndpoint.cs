using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.sample
{
    public class DeleteFilesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapDelete("/files", (HttpRequest request) =>
                {
                    return WellKnownStateHandler.HandleWellKnownState(GetState(request));
                })
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Deletes a file", 3));
                    d.Append(new MarkdownParagraph("Just a demo method, won't actually delete anything"));
                });
        }
    }
}
