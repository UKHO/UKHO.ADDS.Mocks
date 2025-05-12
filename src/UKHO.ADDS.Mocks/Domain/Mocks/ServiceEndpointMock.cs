using UKHO.ADDS.Mocks.Domain.Configuration;
using UKHO.ADDS.Mocks.States;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks
{
    public abstract class ServiceEndpointMock
    {
        internal const string PerRequestHeaderKey = "x-addsmockstate";
        internal const string PerEndpointHeaderKey = "x-addsmockstateendpoint";

        private ServiceDefinition? _definition;

        public abstract void RegisterSingleEndpoint(IEndpointMock endpoint);

        protected string GetState(HttpRequest request)
        {
            if (_definition == null)
            {
                return WellKnownState.Default;
            }

            // Check per-session (developer-set, flows)
            var sessionId = request.Headers.TryGetValue(PerEndpointHeaderKey, out var sessionHeader)
                ? sessionHeader.ToString()
                : "interactive";

            var callerType = GetType().Name;
            var prefix = _definition.Prefix;
            var key = $"{sessionId}/{prefix}/{callerType}";

            if (_definition.StateOverrides.TryGetValue(key, out var sessionOverride))
            {
                return sessionOverride;
            }

            // Check per-request (unit tests)
            if (request.Headers.TryGetValue(PerRequestHeaderKey, out var perRequestValue))
            {
                return perRequestValue!;
            }

            return WellKnownState.Default;
        }

        internal void SetDefinition(ServiceDefinition definition) => _definition = definition;
    }
}
