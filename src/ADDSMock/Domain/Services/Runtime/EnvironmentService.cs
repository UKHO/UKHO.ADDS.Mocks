using ADDSMock.Domain.Configuration;

namespace ADDSMock.Domain.Services.Runtime
{
    internal class EnvironmentService : IEnvironmentService
    {
        public EnvironmentService(MockConfiguration mockConfiguration, ServiceConfigurationCollection serviceConfigurations, bool isInteractive)
        {
            Mock = mockConfiguration;
            Services = serviceConfigurations;
            IsInteractive = isInteractive;
        }

        public MockConfiguration Mock { get; }

        public ServiceConfigurationCollection Services { get; }

        public bool IsInteractive { get; }
    }
}
