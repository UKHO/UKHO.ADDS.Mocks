using UKHO.ADDS.Mocks.Domain.Configuration;
using UKHO.ADDS.Mocks.Domain.Internal.Logging;
using UKHO.ADDS.Mocks.States;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks
{
    public abstract class ServiceEndpointMock
    {
        internal const string PerRequestHeaderKey = "x-addsmockstate";
        internal const string PerEndpointHeaderKey = "x-addsmockstateendpoint";

        private ServiceDefinition? _definition;
        private ILogger<ServiceEndpointMock>? _logger;

        public abstract void RegisterSingleEndpoint(IEndpointMock endpoint);

        protected string GetState(HttpRequest request)
        {
            if (_definition == null)
            {
                return WellKnownState.Default;
            }

            var endpointName = GetType().Name;
            var prefix = _definition.Prefix;

            // Check per-session (developer-set, flows)
            var sessionId = request.Headers.TryGetValue(PerEndpointHeaderKey, out var sessionHeader)
                ? sessionHeader.ToString()
                : "interactive";

            var key = $"{sessionId}/{prefix}/{endpointName}";

            if (_definition.StateOverrides.TryGetValue(key, out var perSessionValue))
            {
                _logger.LogStateSelected(new StateLogView(endpointName, prefix, sessionId, perSessionValue, StateSelectionMode.PerRequest));
                return perSessionValue;
            }

            // Check per-request (unit tests)
            if (request.Headers.TryGetValue(PerRequestHeaderKey, out var perRequestValue))
            {
                _logger.LogStateSelected(new StateLogView(endpointName, prefix, string.Empty, perRequestValue, StateSelectionMode.PerRequest));
                return perRequestValue;
            }

            _logger.LogStateSelected(new StateLogView(endpointName, prefix, string.Empty, WellKnownState.Default, StateSelectionMode.Default));
            return WellKnownState.Default;
        }

        internal void SetDefinition(ServiceDefinition definition) => _definition = definition;

        internal void SetLogger(ILogger<ServiceEndpointMock> logger) => _logger = logger;
    }
}
