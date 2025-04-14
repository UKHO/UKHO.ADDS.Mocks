namespace UKHO.ADDS.Mocks.Domain.Configuration
{
    public sealed class ServiceDefinition
    {
        private readonly string _name;
        private readonly string _prefix;
        private readonly List<ServiceFile> _serviceFiles;

        private readonly List<ServiceFragment> _serviceFragments;

        private string? _error;

        public ServiceDefinition(string prefix, string name)
        {
            _name = name;
            _prefix = prefix;

            _serviceFiles = [];
            _serviceFragments = [];
        }

        public string Prefix => _prefix;

        public string Name => _name;

        internal bool HasError => !string.IsNullOrEmpty(_error);

        internal string Error => _error ?? string.Empty;

        internal IEnumerable<ServiceFragment> ServiceFragments => _serviceFragments;

        internal IEnumerable<ServiceFile> ServiceFiles => _serviceFiles;

        internal void SetError(string error) => _error = error;

        public void AddServiceMockTypes(IDictionary<string, (Type type, bool isOverride)> serviceMockTypes)
        {
            foreach (var serviceMockType in serviceMockTypes)
            {
                var fragment = new ServiceFragment(this, serviceMockType.Key, serviceMockType.Value.type, serviceMockType.Value.isOverride);
                _serviceFragments.Add(fragment);
            }
        }

        public void AddFilePaths(IDictionary<string, (string path, bool isOverride)> serviceFiles)
        {
            foreach (var serviceFile in serviceFiles)
            {
                var file = new ServiceFile(serviceFile.Key, serviceFile.Value.path, serviceFile.Value.isOverride);
                _serviceFiles.Add(file);
            }
        }
    }
}
