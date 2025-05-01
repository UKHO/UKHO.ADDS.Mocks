using UKHO.ADDS.Mocks.Domain.Configuration;
using UKHO.ADDS.Mocks.States;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks
{
    public abstract class ServiceEndpointMock
    {
        private ServiceDefinition? _definition;
        internal const string HeaderKey = "x-addsmockstate";

        public abstract void RegisterSingleEndpoint(IEndpointMock endpoint);

        protected string GetState(HttpRequest request)
        {
            if (_definition!.DefaultState != WellKnownState.Default)
            {
                return _definition.DefaultState;
            }

            if (request.Headers.TryGetValue(HeaderKey, out var value))
            {
                return value!;
            }

            return WellKnownState.Default;
        }

        internal void SetDefinition(ServiceDefinition definition)
        {
            _definition = definition;
        }
    }
}
