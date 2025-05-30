namespace UKHO.ADDS.Mocks.Api.Models.Traffic
{
    internal class ResponseModel<TBody>
    {
        public int StatusCode { get; init; }
        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
        public TBody Body { get; init; } = default!;
    }
}
