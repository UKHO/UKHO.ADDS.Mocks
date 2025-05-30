using UKHO.ADDS.Mocks.Api.Models.Traffic;
using UKHO.ADDS.Mocks.Dashboard.Services;

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

                var requestResponses = dashboardService.RequestResponses.Where(x => x.Timestamp >= since).TakeLast(limit.Value).Select(RequestResponseModelBuilder.Build);

                return requestResponses.ToList();
            });

            adminEndpoint.MapDelete("/requests", (HttpRequest request, DashboardService dashboardService) => { dashboardService.RequestResponses.Clear(); });
        }
    }
}
