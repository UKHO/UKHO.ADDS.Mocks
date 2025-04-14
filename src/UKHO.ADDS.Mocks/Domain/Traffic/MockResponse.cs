namespace UKHO.ADDS.Mocks.Domain.Traffic
{
    internal class MockResponse
    {
        public int StatusCode { get; init; }
        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
        public byte[] Body { get; init; } = Array.Empty<byte>();
    }
}
