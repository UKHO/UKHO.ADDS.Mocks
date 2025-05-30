namespace UKHO.ADDS.Mocks.Api.Models.Traffic
{
    internal class RequestModel<TBody>
    {
        public string Method { get; init; } = default!;
        public string Path { get; init; } = default!;
        public string QueryString { get; init; } = default!;
        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
        public TBody Body { get; init; } = default!;
    }
}
