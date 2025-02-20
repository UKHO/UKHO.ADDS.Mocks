namespace ADDSMock.Domain.Mappings
{
    public class MockServiceFile
    {
        private readonly string _name;
        private readonly string _path;
        private readonly string _mimeType;
        private readonly bool _isOverride;

        public MockServiceFile(string path, string name, string mimeType, bool isOverride)
        {
            _name = name;
            _path = path;
            _mimeType = mimeType;
            _isOverride = isOverride;
        }

        public string Name => _name;

        public string Path => _path;

        public string MimeType => _mimeType;

        public bool IsOverride => _isOverride;
    }
}
