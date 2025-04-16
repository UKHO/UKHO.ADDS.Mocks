using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class GetFssBatchesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapGet("/batch", (HttpRequest request) =>
            {
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:

                        var pathResult = endpoint.GetFile("annualfiles.json");

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
                d.Bold("Gets Batchs")
                    .AppendNewLine()
                    .Italic("This is driven from a static file annualfiles.json")
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
