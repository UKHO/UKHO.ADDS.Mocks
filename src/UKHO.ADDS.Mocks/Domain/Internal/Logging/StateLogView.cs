namespace UKHO.ADDS.Mocks.Domain.Internal.Logging
{
    internal class StateLogView
    {
        public StateLogView(string endpointName, string prefix, string sessionId, string state, StateSelectionMode mode)
        {
            EndpointName = endpointName;
            Prefix = prefix;
            SessionId = sessionId;
            State = state;
            Mode = mode;
        }

        private StateSelectionMode Mode { get; init; }

        public string EndpointName { get; init; }
        public string Prefix { get; init; }

        public string SessionId { get; init; }

        public string State { get; init; }

    }
}
