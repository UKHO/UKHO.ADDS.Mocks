using LightResults;
using WireMock.Server;

namespace ADDSMock.Domain.Services
{
    public interface IWireMockService
    {
        WireMockServer? WireMockServer { get; }

        string BaseUrl { get; }

        Result Start();

        void Stop();
    }
}
