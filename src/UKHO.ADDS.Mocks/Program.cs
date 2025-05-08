namespace UKHO.ADDS.Mocks
{
    internal class Program
    {
        private static async Task Main(string[] args) => await MockServer.RunAsync(args);
    }
}
