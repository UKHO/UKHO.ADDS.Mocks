using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UKHO.ADDS.Mocks.Configuration.Mocks.ees.Models;
using UKHO.ADDS.Mocks.Markdown;

namespace UKHO.ADDS.Mocks.EES.Override.Mocks.ees
{
    public class PublishEventEndpoint : ServiceEndpointMock
    {
        ILogger<PublishEventEndpoint> _logger;

        public PublishEventEndpoint(ILogger<PublishEventEndpoint> logger)
        {
            _logger = logger;
        }
        public void RegisterSingleEndpoint1(IEndpointMock endpoint) =>       
            endpoint.MapPost("/api/events", async (HttpRequest request, ILogger<PublishEventEndpoint> logger) =>
                {
                    var state = GetState(request);

                    await HandlePostAsync(request, state);
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


        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPost("/api/events",  (HttpRequest request, ILogger<PublishEventEndpoint> logger) =>
            {
                var state = GetState(request);
               
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

        private async Task<IActionResult> HandlePostAsync(HttpRequest request, string state)
        {
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = reader.ReadToEnd();
            request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(body))
            {
                return new BadRequestResult();
            }

            var model = JsonSerializer.Deserialize<CloudEventExtension>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (!model.cloudEvent.IsValid)
            {
                _logger.LogWarning("{cloudEvent} is not valid", model.cloudEvent);
                return new BadRequestResult();
            }
            _logger.LogInformation("Event received for {cloudEvent}", model.cloudEvent);


            if (!ValidateCloudEventContents(model.cloudEvent.Type, model.cloudEvent.Data, state))
            {
                _logger.LogWarning("{cloudEvent} is not valid", model.cloudEvent);
                return new BadRequestResult();
            }

            _logger.LogInformation("Cloud Event passed schema validation {cloudEvent}", model.cloudEvent);

           if(state == "eventgrid-failure")
            {
                _logger.LogError("Simulated Event Grid failure for {cloudEvent}", model.cloudEvent);
                return new ObjectResult("") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            // Simulate success
            await Task.Delay(50);

            _logger.LogInformation("Event sent successfully {cloudEvent}", model.cloudEvent);

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
                _logger.LogWarning("Schema validation failed for {eventName}, reason: {errors}", eventName, "Cloud data is empty.");
                return false;
            }
            if(state == "post-invalid-schema")
            {
                _logger.LogWarning("Schema validation failed for {eventName}, reason: {errors}", eventName, "Simulated invalid schema.");
                return false;
            }

            return true;
        }
    }

}
