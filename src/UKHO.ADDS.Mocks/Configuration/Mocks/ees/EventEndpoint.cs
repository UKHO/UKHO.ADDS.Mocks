using System.Net;
using Microsoft.AspNetCore.Mvc;
using UKHO.ADDS.Mocks.Configuration.Mocks.ees.Models;
using UKHO.ADDS.Mocks.Markdown;


namespace UKHO.ADDS.Mocks.EES.Override.Mocks.ees
{
    public class EventEndpoint : ServiceEndpointMock
    {
        public ILogger<EventEndpoint> _logger;

       //public EventEndpoint(ILogger<EventEndpoint> logger)
       // {
       //     _logger = logger;
       // }
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapPost("/api/event", async([FromBody] CloudEventExtension model, HttpRequest request, ILogger<EventEndpoint> logger
                    ) =>
                {
                    var state = GetState(request);

                    await HandlePostAsync(model, state, logger);
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

        private async Task<IActionResult> HandlePostAsync(CloudEventExtension model, string state, ILogger<EventEndpoint> logger)
        {

            if (!model.cloudEvent.IsValid)
            {
                logger.LogWarning("{cloudEvent} is not valid", model.cloudEvent);
                return new BadRequestResult();
            }
            logger.LogInformation("Event received for {cloudEvent}", model.cloudEvent);


            if (!ValidateCloudEventContents(model.cloudEvent.Type, model.cloudEvent.Data, state))
            {
                logger.LogWarning("{cloudEvent} is not valid", model.cloudEvent);
                return new BadRequestResult();
            }

            logger.LogInformation("Cloud Event passed schema validation {cloudEvent}", model.cloudEvent);

            if (state == "eventgrid-failure")
            {
                logger.LogError("Simulated Event Grid failure for {cloudEvent}", model.cloudEvent);
                return new ObjectResult("") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            // Simulate success
            await Task.Delay(50);

            logger.LogInformation("Event sent successfully {cloudEvent}", model.cloudEvent);

            return new OkObjectResult("");
        }
        bool ValidateCloudEventContents(string eventName, object data, string state)
        {

            //var schema = _schemaService.GetSchema(eventName ?? string.Empty);

            //if (schema == null)
            //{
            //    return false;
            //}
            if (data == null)
            {
              //  logger.LogWarning("Schema validation failed for {eventName}, reason: {errors}", eventName, "Cloud data is empty.");
                return false;
            }
            if (state == "post-invalid-schema")
            {
               // logger.LogWarning("Schema validation failed for {eventName}, reason: {errors}", eventName, "Simulated invalid schema.");
                return false;
            }

            return true;
        }   
}
}
