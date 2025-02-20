using WireMock.Admin.Mappings;

namespace ADDSMock.Domain.Mappings
{
    public class MockServiceFragment
    {
        private readonly string _fragmentName;
        private readonly string _content;
        private readonly bool _isOverride;

        private readonly List<MappingModel> _mappings;

        private string? _error;

        public MockServiceFragment(string fragmentName, string content, bool isOverride)
        {
            _fragmentName = fragmentName;
            _content = content;
            _isOverride = isOverride;

            _mappings = [];
        }

        public string FragmentName => _fragmentName;

        public string Content => _content;

        public bool IsOverride => _isOverride;

        public string? Error => _error;

        public bool HasError => !string.IsNullOrEmpty(_error);

        public IEnumerable<MappingModel> Mappings => _mappings;

        public void AddMappings(IEnumerable<MappingModel> addedMappings)
        {
            _mappings.AddRange(addedMappings);
        }

        public void SetError(string error)
        {
            _error = error;
        }
    }
}
