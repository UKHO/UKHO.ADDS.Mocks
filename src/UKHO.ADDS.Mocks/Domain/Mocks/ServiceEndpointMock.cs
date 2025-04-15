using UKHO.ADDS.Mocks.States;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks
{
    public abstract class ServiceEndpointMock
    {
        internal const string HeaderKey = "x-addsmockstate";

        public abstract void RegisterSingleEndpoint(IEndpointMock endpoint);

        protected string GetState(HttpRequest request)
        {
            if (request.Headers.TryGetValue(HeaderKey, out var value))
            {
                return value!;
            }

            return WellKnownState.Default;
        }
    }
}
