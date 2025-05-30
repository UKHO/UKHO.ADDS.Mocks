namespace UKHO.ADDS.Mocks.Api.Models.Traffic
{
    internal class RequestResponseModel<TRequestBody, TResponseBody>
    {
        public DateTime Timestamp { get; init; }
        public RequestModel<TRequestBody> Request { get; init; } = default!;
        public ResponseModel<TResponseBody> Response { get; init; } = default!;
    }
}
