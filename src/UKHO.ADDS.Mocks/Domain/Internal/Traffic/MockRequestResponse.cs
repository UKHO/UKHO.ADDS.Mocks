namespace UKHO.ADDS.Mocks.Domain.Internal.Traffic
{
    internal class MockRequestResponse
    {
        public DateTime Timestamp { get; set; }
        public MockRequest Request { get; set; } = default!;
        public MockResponse Response { get; set; } = default!;
    }
}
