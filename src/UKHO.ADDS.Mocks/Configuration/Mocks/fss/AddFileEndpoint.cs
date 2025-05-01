using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class AddFileEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapPost("/batch/{batchId}/files/{fileName}", (string batchId, string fileName,HttpRequest request) =>
            {
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:

                        return Results.Created();

                    default:
                        // Just send default responses
                        return WellKnownStateHandler.HandleWellKnownState(state);
                }
            })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Bold("Add a file")
                        .AppendNewLine()
                        .Italic("Just returns a 201, won't actually create anything")
                        .AppendNewLine()
                        .Append("Requires a batchId & fileName.");
                });
        }
    }
}
