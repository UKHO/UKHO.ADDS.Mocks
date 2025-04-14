using UKHO.ADDS.Mocks.Configuration;

namespace UKHO.ADDS.Mocks.SampleService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ServiceRegistry.AddDefinitionState("sample", "really-get-jpeg");

            await MockServer.RunAsync(args);
        }
    }
}
