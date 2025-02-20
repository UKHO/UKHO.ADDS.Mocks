using ADDSMock.Domain.Mappings;
using LightResults;

namespace ADDSMock.Domain.Services
{
    public interface IMappingService
    {
        Task<Result> ReadMappingsAsync();

        Task<Result> ExecuteMappingsAsync(IWireMockService wireMockService);

        IEnumerable<MockService> Services { get; }

        Task ReloadMappingsAsync(IWireMockService wireMockService);
    }
}
