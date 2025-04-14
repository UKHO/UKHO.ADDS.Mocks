using UKHO.ADDS.Mocks.Domain.Configuration;

namespace UKHO.ADDS.Mocks.Configuration
{
    public static class ServiceRegistry
    {
        private static readonly List<ServiceDefinition> _definitions =
        [
            new("sample", "Sample Service", new [] {"really-get-file"})
        ];

        public static IEnumerable<ServiceDefinition> Definitions => _definitions;

        public static void AddDefinition(ServiceDefinition definition)
        {
            _definitions.Add(definition);
        }

        public static void AddDefinitionState(string definitionPrefix, string state)
        {
            var definition = _definitions.FirstOrDefault(d => d.Prefix.Equals(definitionPrefix, StringComparison.OrdinalIgnoreCase));
            if (definition != null)
            {
                definition.AddState(state);
            }
        }
    }
}
