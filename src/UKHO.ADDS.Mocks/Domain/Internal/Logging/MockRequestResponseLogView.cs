using UKHO.ADDS.Mocks.Domain.Internal.Traffic;

namespace UKHO.ADDS.Mocks.Domain.Internal.Logging
{
    internal class MockRequestResponseLogView
    {
        public MockRequestResponseLogView(MockRequestResponse log)
        {
            Method = log.Request.Method;
            Path = log.Request.Path;
            QueryString = log.Request.QueryString;
            RequestHeaders = log.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            StatusCode = log.Response.StatusCode;
            ResponseHeaders = log.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        }

        public string Method { get; init; }
        public string Path { get; init; }
        public string QueryString { get; init; }
        public IDictionary<string, string> RequestHeaders { get; init; }

        public int StatusCode { get; init; }
        public IDictionary<string, string> ResponseHeaders { get; init; }
    }
}
