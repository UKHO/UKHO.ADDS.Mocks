using Microsoft.AspNetCore.Mvc;
using UKHO.ADDS.Mocks.Configuration.Mocks.ees.Models;
using UKHO.ADDS.Mocks.Configuration.Mocks.ees.Services;
using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;


namespace UKHO.ADDS.Mocks.EES.Override.Mocks.ees
{
    public class EventEndpoint : ServiceEndpointMock
    {
        public ILogger<EventEndpoint> _logger;

        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapPost("/api/event", async ([FromBody] CloudEventExtension model, HttpRequest request, EESResponseGenerator responseGenerator, ILogger < EventEndpoint> logger
                    ) =>
                {
                    var state = GetState(request);
                    switch (state)
                    {
                        case WellKnownState.Default:
                        case "eventgrid-failure":
                        case "invalid-schema":
                            return await responseGenerator.HandlePostAsync(model, state, logger);

                        default:
                            // Just send default responses
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
                        new MarkdownTextListItem("Rejects when cloudEvent.Id is false"),
                        new MarkdownTextListItem("Rejects when cloudEvent.Type is missing/unknown"),
                        new MarkdownTextListItem("Rejects when cloudEvent.Source is missing"),
                        new MarkdownTextListItem("Rejects when cloudEvent.Data is missing")
                    ));
                });
        }
}
}
