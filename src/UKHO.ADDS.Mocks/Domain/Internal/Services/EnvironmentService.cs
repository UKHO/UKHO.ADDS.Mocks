namespace UKHO.ADDS.Mocks.Domain.Internal.Services
{
    internal class EnvironmentService
    {
        public EnvironmentService(IWebHostEnvironment hostEnvironment)
        {
            IsStandalone = hostEnvironment.ApplicationName.Equals("UKHO.ADDS.Mocks", StringComparison.InvariantCultureIgnoreCase);
            AssetPathPrefix = IsStandalone ? "/" : "_content/UKHO.ADDS.Mocks/";
        }

        public bool IsStandalone { get; }

        public string AssetPathPrefix { get; }
    }
}
