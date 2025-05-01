using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fssmsi
{
    public class GetFssMsiBatchAttributesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapGet("/attributes/search", (HttpRequest request) =>
            {
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:

                        var pathResult = endpoint.GetFile("attributes.json");

                        if (pathResult.IsSuccess(out var file))
                        {
                            return Results.File(file.Path, contentType: MimeType.Application.Json);
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
                d.Bold("Gets Batch Attributes (MSI)")
                    .AppendNewLine()
                    .Italic("This is driven from a static file attributes.json")
                .AppendNewLine()
                .Append("Simulates basic attribute query")
                .AppendNewLine()
                .Append("Please go [here](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-9.0) if you want to know more about minimal APIs")
                .AppendNewLine();
                
            });
        }
    }
}
