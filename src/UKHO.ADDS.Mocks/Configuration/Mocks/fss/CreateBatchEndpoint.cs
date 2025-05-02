using UKHO.ADDS.Mocks.Markdown;
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
                        return WellKnownStateHandler.HandleWellKnownState(state);
                }
            })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Create a batch", 3));
                    d.Append(new MarkdownParagraph("Just returns a 201, won't actually create anything"));
                });
        }
    }
}
