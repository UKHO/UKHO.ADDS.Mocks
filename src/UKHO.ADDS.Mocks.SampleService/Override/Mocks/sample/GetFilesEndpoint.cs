using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.SampleService.Override.Mocks.sample
{
    public class GetFilesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapGet("/files", (HttpRequest request) =>
            {
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:
                        // ADDS Mock will have the 'default' state unless we have told it otherwise
                        return Results.Ok("This is a result, just needed this text with the 200 response");

                    case "get-file":

                        var pathResult = endpoint.GetFile("readme.txt");

                        if (pathResult.IsSuccess(out var file))
                        {
                            return Results.File(file.Path, contentType: MimeType.Text.Plain);
                        }

                        return Results.NotFound("Could not find the path in the /files GET method");

                    case "get-jpeg":

                        var jpegPathResult = endpoint.GetFile("messier-78.jpg");

                        if (jpegPathResult.IsSuccess(out var jpegFile))
                        {
                            return Results.File(jpegFile.Path, contentType: MimeType.Image.Jpeg);
                        }

                        return Results.NotFound("Could not find the JPEG path in the /files GET method");


                    default:
                        // Just send default responses
                        return WellKnownStateHandler.HandleWellKnownState(state);
                }
            })
            .WithEndpointMetadata(endpoint, d =>
            {
                d.Bold("Gets a file, with some extra states")
                    .AppendNewLine()
                    .Italic("Just a demo method, nothing too exciting")
                    .AppendNewLine()
                    .Append("This is a description")
                    .AppendNewLine()
                    .Append("Please go [here](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-9.0) if you want to know more about minimal APIs")
                    .AppendNewLine()
                    .Append("This is a list of things:")
                    .Append("- Thing 1")
                    .Append("- Thing 2")
                    .Append("- Thing 3");
            });
        }
    }
}
