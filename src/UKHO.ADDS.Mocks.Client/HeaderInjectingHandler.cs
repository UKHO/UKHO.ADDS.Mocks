namespace UKHO.ADDS.Mocks.Client
{
    internal class HeaderInjectingHandler : DelegatingHandler
    {
        private readonly string _headerName;
        private readonly Func<string> _headerValueFunc;

        public HeaderInjectingHandler(string headerName, Func<string> headerValueFunc)
        {
            _headerName = headerName;
            _headerValueFunc = headerValueFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(_headerName))
            {
                request.Headers.Add(_headerName, _headerValueFunc());
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
