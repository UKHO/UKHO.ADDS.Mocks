using ADDSMock.Domain.Configuration;

namespace ADDSMock.Domain.Mappings
{
    public class MockService
    {
        private readonly string _servicePrefix;
        private readonly string _serviceName;
        private readonly List<MockServiceFragment> _fragments;
        private readonly List<MockServiceFile> _files;
        private MockServiceConfiguration? _configuration;

        public MockService(string servicePrefix, string serviceName)
        {
            _servicePrefix = servicePrefix;
            _serviceName = serviceName;
            _fragments = [];
            _files = [];
        }

        public string ServicePrefix => _servicePrefix;

        public string ServiceName => _serviceName;

        public IEnumerable<MockServiceFragment> Fragments => _fragments;

        public MockServiceConfiguration Configuration => _configuration;

        public IEnumerable<MockServiceFile> Files => _files;

        internal void AddFragments(IEnumerable<MockServiceFragment> fragments)
        {
            _fragments.AddRange(fragments);
        }

        internal void AddConfiguration(MockServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddFiles(IEnumerable<MockServiceFile> files)
        {
            _files.AddRange(files);
        }
    }
}
