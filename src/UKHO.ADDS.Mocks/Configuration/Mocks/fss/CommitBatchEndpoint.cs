using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class CommitBatchEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPut("/batch/{batchId}", (string batchId, HttpRequest request) =>
            {
                var state = GetState(request);

                var rawRequestBody = new StreamReader(request.Body).ReadToEnd();
                if (string.IsNullOrEmpty(rawRequestBody))
                {
                      var errorObj = new
                      {
                          message = "Body Required with one or more",
                          Files = new[]
                          {
                              new { FileName = "testfilename", Hash = "wDgpHouMMmN7CIWrSaZxsQ==" }
                          }
                      };

                    return Results.BadRequest(errorObj);
                }

                switch (state)
                {
                    case WellKnownState.Default:
                        var result = new { uri = $"/batch/{batchId}/status" };
                        return Results.Accepted("",result);

                    default:
                        // Just send default responses
                        return WellKnownStateHandler.HandleWellKnownState(state);
                }
            })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Commits a batch", 3));
                    d.Append(new MarkdownParagraph("Just returns a 202, won't actually commit anything"));
                });

    }
}
