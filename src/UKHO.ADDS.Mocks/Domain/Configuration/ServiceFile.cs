namespace UKHO.ADDS.Mocks.Domain.Configuration
{
    internal class ServiceFile : IServiceFile
    {
        private readonly bool _isOverride;
        private readonly string _name;
        private readonly string _path;

        public ServiceFile(string name, string path, bool isOverride)
        {
            _name = name;
            _path = path;
            _isOverride = isOverride;
        }

        public string Name => _name;

        public string Path => _path;

        public bool IsOverride => _isOverride;
    }
}
