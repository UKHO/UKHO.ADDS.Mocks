using UKHO.ADDS.Infrastructure.Results;
using UKHO.ADDS.Mocks.Domain.Configuration;

namespace UKHO.ADDS.Mocks.Domain.Internal.Configuration
{
    internal class ServiceFragment
    {
        private readonly ServiceDefinition _definition;
        private readonly bool _isOverride;
        private readonly string _name;
        private readonly Type _type;

        public ServiceFragment(ServiceDefinition definition, string name, Type type, bool isOverride)
        {
            _definition = definition;
            _name = name;
            _type = type;
            _isOverride = isOverride;
        }

        public string Name => _name;

        public Type Type => _type;

        public bool IsOverride => _isOverride;

        public string Prefix => _definition.Prefix;

        public string ServiceName => _definition.Name;

        internal ServiceDefinition Definition => _definition;

        public IResult<IServiceFile> GetFilePath(string fileName)
        {
            var file = _definition.ServiceFiles.SingleOrDefault(f => f.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));

            if (file == null)
            {
                return Result.Failure<IServiceFile>($"File '{fileName}' not found for mock service '{_definition.Name}'.");
            }

            return Result.Success<IServiceFile>(file);
        }

        public IEndpointMock CreateBuilder(RouteGroupBuilder groupBuilder) => new EndpointMockBuilder(groupBuilder, this);
    }
}
