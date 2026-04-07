using System.Net;
using UKHO.ADDS.Mocks.Configuration.Mocks.ees.Models;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.ees.Services
{
    public class EESResponseGenerator 
    {
        public Task<IResult> HandlePostAsync(CloudEventExtension model, string state, ILogger<EventEndpoint> logger)
        {
            if (!model.cloudEvent.IsValid)
            {
                logger.LogWarning("cloudEvent is not valid");
                return Task.FromResult(Results.BadRequest("cloudEvent is not valid"));
            }
            logger.LogInformation("Event received for cloudEvent");


            if (!ValidateCloudEventContents(model.cloudEvent.Type, model.cloudEvent.Data, state, logger))
            {
                logger.LogWarning("cloudEvent is not valid");
                return Task.FromResult(Results.BadRequest("cloudEvent is not valid"));
            }

            logger.LogInformation("Cloud Event passed schema validation cloudEvent");

            if (state == "eventgrid-failure")
            {
                logger.LogError("An error occurred publishing to Event Grid. Service returned status code:{eventGridResponseStatusCode}", (int)HttpStatusCode.InternalServerError);
                return Task.FromResult(
                    Results.Problem("Simulated Event Grid failure", statusCode: (int)HttpStatusCode.InternalServerError)
                );
            }
            //Simulate success
            Task.Delay(50);
            logger.LogInformation("Event sent successfully");

            return Task.FromResult(Results.Ok("Event sent successfully"));
        }
        bool ValidateCloudEventContents(string eventName, object data, string state, ILogger<EventEndpoint> logger)
        {

            var schema = SchemaStore.AllowedSchemas.FirstOrDefault(s => string.Equals(s, eventName, StringComparison.OrdinalIgnoreCase));

            if (schema == null)
            {
                return false;
            }
            if (data == null)
            {
                logger.LogWarning("Schema validation failed for {eventName}, reason: {errors}", eventName, "Cloud data is empty.");
                return false;
            }
            if (state == "invalid-schema")
            {
                logger.LogWarning("Schema validation failed for {eventName}, reason: {errors}", eventName, "Simulated invalid schema.");
                return false;
            }

            return true;
        }
    }
}
