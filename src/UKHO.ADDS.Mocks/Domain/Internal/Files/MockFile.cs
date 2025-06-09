using UKHO.ADDS.Mocks.Files;

namespace UKHO.ADDS.Mocks.Domain.Internal.Files
{
    internal class MockFile : IMockFile
    {
        private readonly bool _isOverride;
        private readonly bool _isReadOnly;
        private readonly string _name;
        private readonly string _path;
        private readonly string _mimeType;
        private long _size;

        public MockFile(string name, string path, string mimeType, long size, bool isOverride, bool isReadOnly)
        {
            _name = name;
            _path = path;
            _mimeType = mimeType;
            _size = size;
            _isOverride = isOverride;
            _isReadOnly = isReadOnly;
        }

        public string Name => _name;

        public string MimeType => _mimeType;

        public string Path => _path;

        public long Size
        {
            get => _size;
            internal set => _size = value;
        }

        public bool IsOverride => _isOverride;

        public bool IsReadOnly => _isReadOnly;

        public Stream Open() => Open(FileMode.Open);

        public Stream Open(FileMode mode) => new FileStream(Path, mode);
    }
}
