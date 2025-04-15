using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.sample
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
                        return Results.Ok("This is a result");

                    case "get-file":

                        var pathResult = endpoint.GetFile("readme.txt");

                        if (pathResult.IsSuccess(out var file))
                        {
                            return Results.File(file.Path, contentType: MimeType.Text.Plain);
                        }

                        return Results.NotFound("Could not find the path in the /files GET method");

                    default:
                        // Just send default responses
                        return DefaultStateHandler.HandleDefaultState(state);
                }
            })
            .Produces<string>()
            .WithEndpointMetadata(endpoint, d =>
            {
                d.Bold("Gets a file")
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
