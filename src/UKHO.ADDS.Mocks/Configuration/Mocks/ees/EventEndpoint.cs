using Microsoft.AspNetCore.Mvc;
using UKHO.ADDS.Mocks.Configuration.Mocks.ees.Models;
using UKHO.ADDS.Mocks.Dashboard;
using UKHO.ADDS.Mocks.EES.Configuration.Mocks.ees.ResponseGenerator;
using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.EES.Override.Mocks.ees
{
    public class EventEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapPost("/api/event", (HttpRequest request, ILogger<EventEndpoint> logger,
                    [FromBody] CloudEventExtension model) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                        case "post-valid-event":
                            if (!CloudEventValidator.ValidateCloudEvent(model))
                            {
                                logger.LogWarning("Cloud event is not valid");
                                return Results.BadRequest();
                            }

                            if (!CloudEventValidator.ValidateCloudEventContents(model.Type, model.Data))
                            {
                                logger.LogWarning("Schema validation failed for type {Type}", model.Type);
                                return Results.BadRequest();
                            }

                            logger.LogInformation("Cloud Event passed schema validation for type {Type}", model.Type);
                            return Results.Ok();

                        case "post-invalid-event":
                            return Results.BadRequest();

                        case "post-invalid-schema":
                            return Results.BadRequest();

                        default:
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Publishes an event", 3));
                    d.Append(new MarkdownParagraph("Validates CloudEvent payload and schema."));
                });
        }
    }
}
