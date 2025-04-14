namespace UKHO.ADDS.Mocks.Domain.Configuration
{
    public static class ServiceRegistry
    {
        private static readonly List<ServiceDefinition> _definitions =
        [
            new("sample", "Sample Service")
        ];

        public static IEnumerable<ServiceDefinition> Definitions => _definitions;

        public static void AddDefinition(ServiceDefinition definition) => _definitions.Add(definition);
    }
}
