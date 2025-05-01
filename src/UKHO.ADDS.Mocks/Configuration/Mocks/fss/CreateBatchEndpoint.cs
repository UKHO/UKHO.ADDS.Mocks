using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class CreateBatchEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapPost("/batch", (HttpRequest request) =>
            {
                var state = GetState(request);

                var rawRequestBody = new StreamReader(request.Body).ReadToEnd();
                if (string.IsNullOrEmpty(rawRequestBody))
                {
                    return Results.BadRequest("Body required");
                }

                switch (state)
                {
                    case WellKnownState.Default:

                        var newBatch   = new
                        {
                            batchId = Guid.NewGuid().ToString()
                        };
                        return Results.Created("",newBatch);

                    default:
                        // Just send default responses
                        return DefaultStateHandler.HandleDefaultState(state);
                }
            })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Bold("Create a batch")
                        .AppendNewLine()
                        .Italic("Just returns a 201, won't actually create anything")
                        .AppendNewLine()
                        .Append("Requires a json formatted body.");
                });
        }
    }
}
