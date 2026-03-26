using UKHO.ADDS.Mocks.Dashboard;
using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.EES.Override.Mocks.ees
{
    public class PublishEventEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapPost("/post-event", (HttpRequest request) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                        case "post-valid-event":
                            return Results.Ok("Cloud event published!");

                        case "post-invalid-event":

                            return Results.BadRequest("Invalid Cloud event");

                        case "post-invalid-schema":

                            return Results.BadRequest("Invalid schema");

                        default:
                            // Just send default responses
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Publishes an event to the grid", 3));
                    d.Append(new MarkdownParagraph("Just returns a 200"));
                });
        }
    }
}
