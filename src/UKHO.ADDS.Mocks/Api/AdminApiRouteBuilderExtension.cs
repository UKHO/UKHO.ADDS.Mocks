using UKHO.ADDS.Mocks.Api.Models.Traffic;
using UKHO.ADDS.Mocks.Configuration;
using UKHO.ADDS.Mocks.Dashboard.Services;
using UKHO.ADDS.Mocks.Domain.Internal.Services;

namespace UKHO.ADDS.Mocks.Api
{
    internal static class AdminApiRouteBuilderExtension
    {
        public static void RegisterAdminApi(this IEndpointRouteBuilder routeBuilder)
        {
            var adminEndpoint = routeBuilder.MapGroup("/_admin").WithTags("Admin API");

            adminEndpoint.MapGet("/requests", (HttpRequest request, DashboardService dashboardService, int? limit, DateTime? since) =>
            {
                limit ??= int.MaxValue;
                since ??= DateTime.MinValue;

                var requestResponses = dashboardService.GetSnapshot().Where(x => x.Timestamp >= since).TakeLast(limit.Value).Select(RequestResponseModelBuilder.Build);

                return requestResponses.ToList();
            });

            adminEndpoint.MapDelete("/requests", (HttpRequest request, DashboardService dashboardService) => { dashboardService.Clear(); });

            adminEndpoint.MapPost("/states/endpoints/{sessionId}/{servicePrefix}/{endpointName}/{state}", (HttpRequest request, string sessionId, string servicePrefix, string endpointName, string state) =>
            {
                var definition = ServiceRegistry.Definitions.FirstOrDefault(d => d.Prefix.Equals(servicePrefix, StringComparison.InvariantCultureIgnoreCase));

                if (definition == null)
                {
                    return Results.NotFound($"Service definition with prefix '{servicePrefix}' not found.");
                }

                definition.AddStateOverride(sessionId, servicePrefix, endpointName, state);
                return Results.Ok();
            });

            adminEndpoint.MapDelete("/states/endpoints/{sessionId}", (HttpRequest request, string sessionId) =>
            {
                foreach (var definition in ServiceRegistry.Definitions)
                {
                    definition.RemoveStateOverride(sessionId);
                }

                return Results.Ok();
            });

            adminEndpoint.MapGet("/files/{servicePrefix}/{filename}", (HttpRequest request, string servicePrefix, string filename, FileService fileService) =>
            {
                var definition = ServiceRegistry.Definitions.FirstOrDefault(d => d.Prefix.Equals(servicePrefix, StringComparison.InvariantCultureIgnoreCase));

                if (definition == null)
                {
                    return Results.NotFound($"Service definition with prefix '{servicePrefix}' not found.");
                }

                var fileResult = fileService.GetFile(definition, filename);

                if (fileResult.IsSuccess(out var file))
                {
                    return Results.File(file.Open(), file.MimeType);
                }

                if (fileResult.IsFailure(out var errorMessage))
                {
                    return Results.BadRequest(errorMessage);
                }

                return Results.Ok();
            });
        }
    }
}
