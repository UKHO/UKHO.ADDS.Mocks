using UKHO.ADDS.Mocks.Domain.Mocks;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.sample
{
    public class BasicEndpoints : ServiceEndpointMock
    {
        public override void RegisterEndpoint(IServiceMockBuilder builder)
        {
            builder.MapGet("/hello", () => Results.Ok("This is a result"));
        }
    }
}
