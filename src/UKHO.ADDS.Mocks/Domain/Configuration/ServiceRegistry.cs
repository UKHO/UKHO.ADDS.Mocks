using UKHO.ADDS.Mocks.Domain.Configuration;
using UKHO.ADDS.Mocks.States;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks.Configuration
{
    public static class ServiceRegistry
    {
        private static readonly List<ServiceDefinition> _definitions = [];

        public static IEnumerable<ServiceDefinition> Definitions => _definitions;

        public static void AddDefinition(ServiceDefinition definition)
        {
            if (definition.Prefix.Equals("_dashboard"))
            {
                throw new ArgumentException("Prefix cannot be '_dashboard'.", nameof(definition));
            }

            var existing = _definitions.SingleOrDefault(d => d.Prefix.Equals(definition.Prefix, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                _definitions.Add(definition);
            }
        }

        public static void AddDefinitionState(string definitionPrefix, StateDefinition state)
        {
            var definition = _definitions.FirstOrDefault(d => d.Prefix.Equals(definitionPrefix, StringComparison.OrdinalIgnoreCase));
            if (definition != null)
            {
                definition.AddState(state);
            }
        }
    }
}
