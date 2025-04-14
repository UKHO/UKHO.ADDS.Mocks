namespace UKHO.ADDS.Mocks.Domain.Traffic
{
    internal class MockRequest
    {
        public string Method { get; init; } = default!;
        public string Path { get; init; } = default!;
        public string QueryString { get; init; } = default!;
        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
        public byte[] Body { get; init; } = Array.Empty<byte>();
    }
}
