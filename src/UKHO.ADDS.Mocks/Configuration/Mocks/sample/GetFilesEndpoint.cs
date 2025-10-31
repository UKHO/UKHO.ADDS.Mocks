using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.sample
{
    public class GetFilesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapGet("/files", (HttpRequest request) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                            // ADDS Mock will have the 'default' state unless we have told it otherwise
                            return Results.Ok("This is a result");

                        case "get-file":
                            try
                            {
                                var fs = GetFileSystem();
                                var s = fs.OpenFile("/readme.txt", FileMode.Open, FileAccess.Read);
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
                    d.Append(new MarkdownHeader("Gets a file", 3));
                    d.Append(new MarkdownParagraph("Just a demo method, nothing too exciting"));
                });
    }
}
