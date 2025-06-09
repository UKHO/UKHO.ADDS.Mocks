using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.SampleService.Override.Mocks.sample
{
    public class CreateFilesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPost("/files", (HttpRequest request) =>
                {
                    var newFileName = $"new-file-{Guid.NewGuid():N}.jpg";
                    CreateFile(newFileName, request.Body);

                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                            return Results.Ok();
                    }

                    return WellKnownStateHandler.HandleWellKnownState(state);
                })
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Copies a file", 3));
                    d.Append(new MarkdownParagraph("For the get-jpeg state, copies the file to a new file and returns the new file"));
                });
    }
}
