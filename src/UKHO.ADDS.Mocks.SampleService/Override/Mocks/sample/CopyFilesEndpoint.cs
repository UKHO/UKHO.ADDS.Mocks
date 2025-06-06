using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.SampleService.Override.Mocks.sample
{
    public class CopyFilesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPost("/files/copy", (HttpRequest request) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                            // ADDS Mock will have the 'default' state unless we have told it otherwise
                            return Results.Ok("This is a result, just needed this text with the 200 response");

                        case "get-jpeg":

                            var jpegPathResult = GetFile("messier-78.jpg");

                            if (jpegPathResult.IsSuccess(out var jpegFile))
                            {
                                var newFileName = $"new-file-{Guid.NewGuid():N}.jpg";

                                using (var s = jpegFile.Open())
                                {
                                    CreateFile(newFileName, s);
                                }

                                var newFileResult = GetFile(newFileName);

                                if (newFileResult.IsSuccess(out var newFile))
                                {
                                    return Results.File(newFile.Open(), newFile.MimeType);
                                }
                            }

                            return Results.NotFound("Could not find the JPEG path in the /files GET method");


                        default:
                            // Just send default responses
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Copies a file", 3));
                    d.Append(new MarkdownParagraph("For the get-jpeg state, copies the file to a new file and returns the new file"));
                });
    }
}
