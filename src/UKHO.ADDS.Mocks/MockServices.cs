using UKHO.ADDS.Mocks.Configuration;
using UKHO.ADDS.Mocks.Domain.Configuration;

namespace UKHO.ADDS.Mocks
{
    internal static class MockServices
    {
        public static void AddServices()
        {
            ServiceRegistry.AddDefinition(new ServiceDefinition("sample", "Sample Service", ["get-file"]));
        }
    }
}
