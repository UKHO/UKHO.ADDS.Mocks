using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class AddFileEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapPost("/batch/{batchId}/files/{fileName}", (HttpRequest request) =>
            {
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:
                        // ADDS Mock will have the 'default' state unless we have told it otherwise
                        return Results.Created();

                    default:
                        // Just send default responses
                        return DefaultStateHandler.HandleDefaultState(state);
                }
            })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Bold("Add a file")
                        .AppendNewLine()
                        .Italic("Just returns a 201, won't actually create anything")
                        .AppendNewLine()
                        .Append("Requires a BatchId & FileName.");
                });
        }
    }
}
