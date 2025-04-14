using UKHO.ADDS.Mocks.Domain.Mocks;

namespace UKHO.ADDS.Mocks.SampleService.Override.Mocks.sample
{
    public class BasicEndpoints : ServiceEndpointMock
    {
        public override void RegisterEndpoint(IServiceMockBuilder builder)
        {
            builder.MapGet("/hello",
                    () => Results.Ok("This is a result from the OVERRIDE"))
                .Produces<string>()
                .WithEndpointMetadata(builder, d =>
                {
                    d.Bold("Well, this is bold")
                        .Italic("And this is italic")
                        .AppendLine()
                        .AppendLine("This is a description")
                        .AppendLine("This is a description with a link to [Google](https://www.google.com)")
                        .AppendLine()
                        .AppendLine("This is a list:")
                        .AppendLine("- Item 1")
                        .AppendLine("- Item 2")
                        .AppendLine("- Item 3");
                });
        }
    }
}
