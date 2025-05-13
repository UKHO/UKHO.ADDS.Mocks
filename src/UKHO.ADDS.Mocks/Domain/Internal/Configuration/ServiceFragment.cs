using System.Collections.Concurrent;
using UKHO.ADDS.Infrastructure.Results;
using UKHO.ADDS.Mocks.Domain.Configuration;

namespace UKHO.ADDS.Mocks.Domain.Internal.Configuration
{
    internal class ServiceFragment
    {
        private readonly bool _isOverride;
        private readonly string _name;
        private readonly Type _type;

        private readonly ConcurrentBag<MappingInfo> _mappings;

        public ServiceFragment(ServiceDefinition definition, string name, Type type, bool isOverride)
        {
            Definition = definition;
            _name = name;
            _type = type;
            _isOverride = isOverride;

            _mappings = new ConcurrentBag<MappingInfo>();
        }

        public string Name => _name;

        public Type Type => _type;

        public bool IsOverride => _isOverride;

        public string Prefix => Definition.Prefix;

        public string ServiceName => Definition.Name;

        internal ServiceDefinition Definition { get; }

        public IEnumerable<MappingInfo> Mappings => _mappings;

        public IResult<IServiceFile> GetFilePath(string fileName)
        {
            var file = Definition.ServiceFiles.SingleOrDefault(f => f.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));

            if (file == null)
            {
                return Result.Failure<IServiceFile>($"File '{fileName}' not found for mock service '{Definition.Name}'.");
            }

            return Result.Success<IServiceFile>(file);
        }

        public IEndpointMock CreateBuilder(RouteGroupBuilder groupBuilder) => new EndpointMockBuilder(groupBuilder, this);

        internal void RecordMapping(string httpMethod, string pattern, string endpointName)
        {
            _mappings.Add(new MappingInfo(httpMethod, pattern, endpointName));
        }
    }
}
