using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class BatchExpiryEndpoint: ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPut("/batch/{batchId}/expiry", (string batchId, HttpRequest request) =>
            {
                var state = GetState(request);

                var rawRequestBody = new StreamReader(request.Body).ReadToEnd();
                if (string.IsNullOrEmpty(rawRequestBody))
                {
                    var errorObj = new
                    {
                        message = "Body Required expiry date",
                        expiryDate = "2022-03-15T12:57:39.896Z"
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
                    d.Append(new MarkdownHeader("Sets batch expiry date", 3));
                    d.Append(new MarkdownParagraph("Just returns a 204, won't actually set expiry"));
                });

    }

}
