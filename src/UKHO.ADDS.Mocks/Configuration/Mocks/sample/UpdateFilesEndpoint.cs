using UKHO.ADDS.Mocks.Domain;
using UKHO.ADDS.Mocks.Domain.Markdown;
using UKHO.ADDS.Mocks.Domain.Mocks;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.sample
{
    public class UpdateFilesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapPut("/files", (HttpRequest request) =>
                {
                    return Results.Ok("File was updated ok (it wasn't really)");
                })
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Updates a file", 3));
                    d.Append(new MarkdownParagraph("Just a demo method, nothing too exciting"));
                });
        }
    }
}
