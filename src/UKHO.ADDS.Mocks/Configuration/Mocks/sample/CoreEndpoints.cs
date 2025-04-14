using UKHO.ADDS.Mocks.Domain.Mocks;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.sample
{
    public class CoreEndpoints : ServiceEndpointMock
    {
        public override void RegisterEndpoint(IServiceMockBuilder builder)
        {
            builder.MapGet("/bonjour", () => Results.Ok("Allo Allo"));
        }
    }
}
