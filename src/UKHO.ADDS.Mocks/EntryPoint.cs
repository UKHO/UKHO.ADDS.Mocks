namespace UKHO.ADDS.Mocks
{
    public class EntryPoint
    {
        private static async Task Main(string[] args) => await MockServer.RunAsync(args);
    }
}
