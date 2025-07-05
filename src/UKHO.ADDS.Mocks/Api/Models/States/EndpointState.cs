
namespace UKHO.ADDS.Mocks.Api.Models.States
{
    public class EndpointStateSetRequest
    {
        public string EndpointId { get; init; }

        public string? ScopeId { get; init; }

        public string State { get; init; }
    }
}
