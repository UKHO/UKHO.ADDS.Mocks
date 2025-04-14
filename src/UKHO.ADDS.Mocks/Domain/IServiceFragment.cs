using UKHO.ADDS.Infrastructure.Results;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks
{
    public interface IServiceFragment
    {
        string Name { get; }

        Type Type { get; }

        bool IsOverride { get; }

        string Prefix { get; }

        string ServiceName { get; }

        IResult<IServiceFile> GetFilePath(string fileName);
    }
}
