using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class UploadBlockEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPut("/batch/{batchId}/files/{fileName}/{blockId}", (string batchId, string filename, string blockId, HttpRequest request) =>
            {
                var state = GetState(request);

                var rawRequestBody = new StreamReader(request.Body).ReadToEnd();

                if (string.IsNullOrEmpty(rawRequestBody))
                {
                    return Results.BadRequest("Body required containing filename");
                }


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
                    d.Append(new MarkdownHeader("Upload a file block", 3));
                    d.Append(new MarkdownParagraph("Just returns a 201, won't actually upload anything"));
                });
    }
}
