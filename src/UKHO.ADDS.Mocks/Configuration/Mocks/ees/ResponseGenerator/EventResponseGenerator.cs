using System.Text.Json;
using System.Text.Json.Nodes;
using UKHO.ADDS.Mocks.Configuration.Mocks.ees.Models;

namespace UKHO.ADDS.Mocks.EES.Configuration.Mocks.ees.ResponseGenerator
{
public static class EventResponseGenerator
{
    private static readonly string _template =
        """
        {
          "status": "Accepted",
          "message": "Cloud event published successfully",
          "eventType": "",
          "isValid": true,
          "receivedAt": "",
          "data": {}
        }
        """;

    public static IResult ProvidePublishEventResponse(HttpRequest request)
    {
        try
        {
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = reader.ReadToEnd();
            request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(body))
            {
                return Results.BadRequest("Invalid request body");
            }

            var model = JsonSerializer.Deserialize<MockCloudEventExtension>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var validationError = ValidateRequest(model);
            if (validationError != null)
            {
                return validationError;
            }

            var jsonTemplate = JsonNode.Parse(_template)?.AsObject();
            UpdateResponseTemplate(jsonTemplate!, model!);

            return Results.Ok(jsonTemplate);
        }
        catch (Exception)
        {
            return Results.InternalServerError("Error occurred while processing event publish request");
        }
    }

    private static IResult? ValidateRequest(MockCloudEventExtension? model)
    {
        if (model == null || model.cloudEvent == null)
        {
            return Results.BadRequest("Invalid request body");
        }

        if (!model.cloudEvent.IsValid)
        {
            return Results.BadRequest("Invalid Cloud event");
        }

        if (string.IsNullOrWhiteSpace(model.cloudEvent.Type) || model.cloudEvent.Data == null)
        {
            return Results.BadRequest("Invalid schema");
        }

        return null;
    }

    private static void UpdateResponseTemplate(JsonObject jsonTemplate, MockCloudEventExtension model)
    {
        jsonTemplate["status"] = "Accepted";
        jsonTemplate["message"] = "Cloud event published successfully";
        jsonTemplate["eventType"] = model.cloudEvent!.Type;
        jsonTemplate["isValid"] = model.cloudEvent.IsValid;
        jsonTemplate["receivedAt"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        jsonTemplate["data"] = JsonSerializer.SerializeToNode(model.cloudEvent.Data);
    }
}
}
