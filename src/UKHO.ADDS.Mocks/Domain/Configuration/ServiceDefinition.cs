using System.Collections.Concurrent;
using UKHO.ADDS.Mocks.Domain.Internal.Configuration;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Domain.Configuration
{
    public sealed class ServiceDefinition
    {
        private readonly string _name;
        private readonly string _prefix;

        private readonly List<ServiceFile> _serviceFiles;
        private readonly List<ServiceFragment> _serviceFragments;
        private readonly List<StateDefinition> _states;

        private readonly ConcurrentDictionary<string, string> _stateOverrides;

        private string? _error;

        public ServiceDefinition(string prefix, string name, IEnumerable<StateDefinition> states)
        {
            _name = name;
            _states = [..states];
            _prefix = prefix;

            _serviceFiles = [];
            _serviceFragments = [];

            _stateOverrides = new ConcurrentDictionary<string, string>();

            _states.Add(new StateDefinition(WellKnownState.Default, "The default state (whatever your endpoint returns without state)"));
            _states.Add(new StateDefinition(WellKnownState.NotFound, "Returns Not Found (404)"));
            _states.Add(new StateDefinition(WellKnownState.NotModified, "Returns Not Modified (304)"));
            _states.Add(new StateDefinition(WellKnownState.BadRequest, "Returns Bad Request (400)"));
            _states.Add(new StateDefinition(WellKnownState.Conflict, "Returns Conflict (409)"));
            _states.Add(new StateDefinition(WellKnownState.Forbidden, "Returns Forbidden (403)"));
            _states.Add(new StateDefinition(WellKnownState.UnsupportedMediaType, "Returns Unsupported Media Type (415)"));
            _states.Add(new StateDefinition(WellKnownState.InternalServerError, "Returns Internal Server Error (500)"));
            _states.Add(new StateDefinition(WellKnownState.Unauthorized, "Returns Unauthorized (401)"));
        }

        public string Prefix => _prefix;

        public string Name => _name;

        internal bool HasError => !string.IsNullOrEmpty(_error);

        internal string Error => _error ?? string.Empty;

        internal IEnumerable<StateDefinition> States => _states;

        internal IEnumerable<string> StateNames => _states.Select(x => x.State);

        internal IEnumerable<ServiceFragment> ServiceFragments => _serviceFragments;

        internal IEnumerable<ServiceFile> ServiceFiles => _serviceFiles;

        internal IReadOnlyDictionary<string, string> StateOverrides => _stateOverrides;

        internal void SetError(string error) => _error = error;

        internal void AddState(string state, string description) => AddState(new StateDefinition(state, description));

        internal void AddState(StateDefinition state) => _states.Insert(0, state);

        internal void AddStateOverride(string sessionId, string prefix, string endpointName, string state)
        {
            var key = $"{sessionId}/{prefix}/{endpointName}";

            if (state == WellKnownState.Default)
            {
                _stateOverrides.Remove(key, out _);
            }
            else
            {
                _stateOverrides[key] = state;
            }
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
