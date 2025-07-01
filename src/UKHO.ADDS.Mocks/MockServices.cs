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

            ServiceRegistry.AddDefinition(new ServiceDefinition("fss", "FileShare Service", []));

            ServiceRegistry.AddDefinition(new ServiceDefinition("fssmsi", "FileShare Service (MSI)", []));

            ServiceRegistry.AddDefinition(new ServiceDefinition("scs", "Sales Catalogue Service", [new StateDefinition("get-missingproducts", "Gets a invalid products")]));

            ServiceRegistry.AddDefinition(new ServiceDefinition("sap", "SAP Service", []));
        }
    }
}
