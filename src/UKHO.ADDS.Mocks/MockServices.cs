using UKHO.ADDS.Mocks.Configuration;
using UKHO.ADDS.Mocks.Domain.Configuration;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks
{
    public static class MockServices
    {
        public static void AddServices()
        {
            ServiceRegistry.AddDefinition(new ServiceDefinition("sample", "Sample Service", [new StateDefinition("get-file", "Gets a plain text file")]));

        }
    }
}
