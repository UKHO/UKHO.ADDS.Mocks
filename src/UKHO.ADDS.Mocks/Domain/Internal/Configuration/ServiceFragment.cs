using System.Collections.Concurrent;
using UKHO.ADDS.Mocks.Configuration;
using UKHO.ADDS.Mocks.Domain.Configuration;

namespace UKHO.ADDS.Mocks.Domain.Internal.Configuration
{
    internal class ServiceFragment 
    {
        private readonly bool _isOverride;
        private readonly string _name;
        private readonly Type _type;

        private readonly ConcurrentBag<EndpointMappingInfo> _mappings;

        public ServiceFragment(ServiceDefinition definition, string name, Type type, bool isOverride)
        {
            Definition = definition;
            _name = name;
            _type = type;
            _isOverride = isOverride;

            _mappings = [];
        }

        public string Name => _name;

        public Type Type => _type;

        public bool IsOverride => _isOverride;

        public string Prefix => Definition.Prefix;

        public string ServiceName => Definition.Name;

        internal ServiceDefinition Definition { get; }

        public IEnumerable<EndpointMappingInfo> Mappings => _mappings;

        public IEndpointMock CreateBuilder(RouteGroupBuilder groupBuilder) => new EndpointMockBuilder(groupBuilder, this);

        internal void RecordMapping(string httpMethod, string pattern, string endpointName)
        {
            _mappings.Add(new EndpointMappingInfo(httpMethod, pattern, endpointName));
        }
    }
}
