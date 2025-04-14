using UKHO.ADDS.Mocks.Configuration;

namespace UKHO.ADDS.Mocks.SampleService
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            ServiceRegistry.AddDefinitionState("sample", "get-jpeg");

            await MockServer.RunAsync(args);
        }
    }
}
