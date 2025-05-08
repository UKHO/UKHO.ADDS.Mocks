// ReSharper disable once CheckNamespace

namespace UKHO.ADDS.Mocks
{
    public interface IServiceFile
    {
        string Name { get; }

        string Path { get; }

        bool IsOverride { get; }
    }
}
