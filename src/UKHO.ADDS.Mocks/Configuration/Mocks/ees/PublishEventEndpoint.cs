using UKHO.ADDS.Mocks.Dashboard;
using UKHO.ADDS.Mocks.EES.Configuration.Mocks.ees.ResponseGenerator;
using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.EES.Override.Mocks.ees
{
    public class PublishEventEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPost("/post-event", (HttpRequest request) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                        case "post-valid-event":
                            return EventResponseGenerator.ProvidePublishEventResponse(request);

                        case "post-invalid-event":
                            return Results.BadRequest("Invalid Cloud event");

                        case "post-invalid-schema":
                            return Results.BadRequest("Invalid schema");

                        default:
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Publishes an event", 3));
                    d.Append(new MarkdownParagraph(
                        new MarkdownEmphasis("This mock validates a CloudEventExtension-style payload.")));

                    d.Append(new MarkdownParagraph("Validation rules:"));
                    d.Append(new MarkdownList(
                        new MarkdownTextListItem("Rejects when cloudEvent.IsValid is false"),
                        new MarkdownTextListItem("Rejects when cloudEvent.Type is missing"),
                        new MarkdownTextListItem("Rejects when cloudEvent.Data is missing")
                    ));
                });
    }
}
