using UKHO.ADDS.Mocks.Domain.Mocks;

namespace UKHO.ADDS.Mocks.SampleService.Override.Mocks.sample
{
    public class ExtraEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IServiceMockBuilder builder)
        {
            builder.MapGet("/extra", () => Results.Ok("Here is my extra")).WithEndpointMetadata(builder, d =>
            {
                d.Bold("Don't ever call this endpoint");
            }); ;
        }
    }
}
