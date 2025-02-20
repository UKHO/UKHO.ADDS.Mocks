using ADDSMock.Domain.Configuration;

namespace ADDSMock.Domain.Services
{
    public interface IEnvironmentService
    {
        MockConfiguration Mock { get; }

        ServiceConfigurationCollection Services { get; }

        bool IsInteractive { get; }
    }
}
