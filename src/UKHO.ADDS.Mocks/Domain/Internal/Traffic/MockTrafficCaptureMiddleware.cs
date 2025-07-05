using UKHO.ADDS.Mocks.Configuration;
using UKHO.ADDS.Mocks.Dashboard.Services;
using UKHO.ADDS.Mocks.Domain.Configuration;

namespace UKHO.ADDS.Mocks.Domain.Internal.Traffic
{
    internal class MockTrafficCaptureMiddleware
    {
        private readonly DashboardService _dashboardService;

        private readonly List<string> _endpointFilters;
        private readonly RequestDelegate _next;

        public MockTrafficCaptureMiddleware(DashboardService dashboardService, RequestDelegate next)
        {
            _dashboardService = dashboardService;
            _next = next;
            _endpointFilters = [];

            var correctDefinitions = ServiceRegistry.Definitions.Where(x => !x.HasError).ToList();

            foreach (var definition in correctDefinitions)
            {
                _endpointFilters.Add($"/{definition.Prefix}/");
            }
        }

        public async Task Invoke(HttpContext context)
        {
            if (_endpointFilters.Any(x => context.Request.Path.ToString().Contains(x)))
            {
                // Capture request
                context.Request.EnableBuffering();

                byte[] requestBody;
                using (var requestBuffer = new MemoryStream())
                {
                    await context.Request.Body.CopyToAsync(requestBuffer);
                    requestBody = requestBuffer.ToArray();
                    context.Request.Body.Position = 0;
                }

                var mockRequest = new MockRequest
                {
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    QueryString = context.Request.QueryString.ToString(),
                    Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                    Body = requestBody
                };

                // Capture response
                var originalResponseBody = context.Response.Body;
                await using var responseBuffer = new MemoryStream();
                context.Response.Body = responseBuffer;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBody = await ReadAllBytesAsync(context.Response.Body);
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                var mockResponse = new MockResponse { StatusCode = context.Response.StatusCode, Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), Body = responseBody };

                // Save captured pair — replace with your own handler later
                var requestResponseModel = new MockRequestResponse { Request = mockRequest, Response = mockResponse, Timestamp = DateTime.UtcNow };
                _dashboardService.AddRequestResponse(requestResponseModel);

                // Write the original response body back to client
                await responseBuffer.CopyToAsync(originalResponseBody);
            }
            else
            {
                await _next(context);
            }
        }

        private static async Task<byte[]> ReadAllBytesAsync(Stream stream)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
