using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class WriteBlockEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPut("/batch/{batchId}/files/{fileName}", (string batchId, string filename, HttpRequest request) =>
            {
                var state = GetState(request);

                var rawRequestBody = new StreamReader(request.Body).ReadToEnd();

                if (string.IsNullOrEmpty(rawRequestBody))
                {
                    var errorObj = new
                    {
                        message = "Body Required with one or more",
                        blockIds = new[]
                          {
                              "00001" 
                          }
                    };

                    return Results.BadRequest(errorObj);
                }

                switch (state)
                {
                    case WellKnownState.Default:

                        return Results.NoContent();



                    default:
                        // Just send default responses
                        return WellKnownStateHandler.HandleWellKnownState(state);
                }
            })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Write a file block", 3));
                    d.Append(new MarkdownParagraph("Just returns a 204, won't actually upload anything"));
                });
    }
}
