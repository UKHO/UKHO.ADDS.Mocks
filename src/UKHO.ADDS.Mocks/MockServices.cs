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
            ServiceRegistry.AddDefinition(new ServiceDefinition("EES", "Enterprise Event Service", [
                new StateDefinition("post-valid-event", "Publish an event"),
                new StateDefinition("post-invalid-event", "Publish an invalid event"),
                new StateDefinition("post-invalid-schema", "Publish an event with invalid schema"),
                ]));
        }
    }
}
