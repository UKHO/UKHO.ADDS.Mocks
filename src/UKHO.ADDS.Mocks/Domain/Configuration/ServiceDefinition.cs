using UKHO.ADDS.Mocks.Domain.Internal.Configuration;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Domain.Configuration
{
    public sealed class ServiceDefinition
    {
        private readonly string _name;
        private readonly List<string> _states;
        private readonly string _prefix;

        private readonly List<ServiceFile> _serviceFiles;
        private readonly List<ServiceFragment> _serviceFragments;

        private string? _error;

        public ServiceDefinition(string prefix, string name, IEnumerable<string> states)
        {
            _name = name;
            _states = [..states];
            _prefix = prefix;

            _serviceFiles = [];
            _serviceFragments = [];

            _states.Add(WellKnownState.Default);
            _states.Add(WellKnownState.NotFound);
            _states.Add(WellKnownState.NotModified);
            _states.Add(WellKnownState.BadRequest);
            _states.Add(WellKnownState.Conflict);
            _states.Add(WellKnownState.Forbidden);
            _states.Add(WellKnownState.InternalServerError);
            _states.Add(WellKnownState.Unauthorized);
        }

        public string Prefix => _prefix;

        public string Name => _name;

        internal bool HasError => !string.IsNullOrEmpty(_error);

        internal string Error => _error ?? string.Empty;

        internal IEnumerable<string> States => _states;

        internal IEnumerable<ServiceFragment> ServiceFragments => _serviceFragments;

        internal IEnumerable<ServiceFile> ServiceFiles => _serviceFiles;

        internal string DefaultState { get; set; } = WellKnownState.Default;

        internal void SetError(string error) => _error = error;

        internal void AddState(string state)
        {
            _states.Insert(0, state);
        }

        internal void AddServiceMockTypes(IDictionary<string, (Type type, bool isOverride)> serviceMockTypes)
        {
            foreach (var serviceMockType in serviceMockTypes)
            {
                var fragment = new ServiceFragment(this, serviceMockType.Key, serviceMockType.Value.type, serviceMockType.Value.isOverride);
                _serviceFragments.Add(fragment);
            }
        }

        internal void AddFilePaths(IDictionary<string, (string path, bool isOverride)> serviceFiles)
        {
            foreach (var serviceFile in serviceFiles)
            {
                var file = new ServiceFile(serviceFile.Key, serviceFile.Value.path, serviceFile.Value.isOverride);
                _serviceFiles.Add(file);
            }
        }
    }
}
