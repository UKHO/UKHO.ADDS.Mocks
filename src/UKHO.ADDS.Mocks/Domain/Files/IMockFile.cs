// ReSharper disable once CheckNamespace

namespace UKHO.ADDS.Mocks.Files
{
    public interface IMockFile
    {
        string Name { get; }

        string MimeType { get; }

        long Size { get; }

        bool IsOverride { get; }

        bool IsReadOnly { get; }

        Stream Open();

        Stream Open(FileMode mode);
    }
}
