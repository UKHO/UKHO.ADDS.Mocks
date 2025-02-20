using ADDSMock.Applications.Interactive.Views.Mappings.Models;

namespace ADDSMock.Applications.Interactive.Views.Mappings.Messages
{
    internal class EndpointsSelectedEvent
    {
        public EndpointsSelectedEvent(IEnumerable<EndpointMappingModel> fragments) => Fragments = fragments;

        public IEnumerable<EndpointMappingModel> Fragments { get; }
    }
}
