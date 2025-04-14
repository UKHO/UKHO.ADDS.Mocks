namespace UKHO.ADDS.Mocks.Domain.Internal.Traffic
{
    internal class MockTrafficCaptureMiddleware
    {
        // You can inject an interface here later for storing the result
        internal static readonly List<(MockRequest, MockResponse)> CapturedTraffic = new();
        private readonly RequestDelegate _next;

        public MockTrafficCaptureMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            // === Request ===
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

            // === Response ===
            var originalResponseBody = context.Response.Body;
            await using var responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;

            await _next(context); // proceed with pipeline

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await ReadAllBytesAsync(context.Response.Body);
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var mockResponse = new MockResponse { StatusCode = context.Response.StatusCode, Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), Body = responseBody };

            // Save captured pair — replace with your own handler later
            CapturedTraffic.Add((mockRequest, mockResponse));

            // Write the original response body back to client
            await responseBuffer.CopyToAsync(originalResponseBody);
        }

        private static async Task<byte[]> ReadAllBytesAsync(Stream stream)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
