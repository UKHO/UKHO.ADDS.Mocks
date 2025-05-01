using UKHO.ADDS.Mocks.SampleService.Override.Mocks.fss.ResponseGenerator;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.SampleService.Override.Mocks.fss
{
    public class GetFssBatchesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapGet("/batch", (HttpRequest request) =>
            {
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:

                        return FssResponseGenerator.ProvideSearchFilterResponse(request);

                    default:
                        // Just send default responses
                        return DefaultStateHandler.HandleDefaultState(state);
                }
            })
            .Produces<string>()
            .WithEndpointMetadata(endpoint, d =>
            {
                d.Bold("Gets Batchs")
                    .AppendNewLine()
                    .Italic("This is driven from Response Generator Process")
                .AppendNewLine()
                .Append("This works with a filter query")
                .AppendNewLine()
                .Append("Please go [here](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-9.0) if you want to know more about minimal APIs")
                .AppendNewLine()
                .Append("Sample query attributes:")
                .Append("- Key $Filter")
                .Append("- Value BusinessUnit eq 'ADDS' and $batch(ProductCode) eq 'AVCS' and  (($batch(ProductName) eq '101GB004DEVQK' and $batch(EditionNumber) eq '2' and (($batch(UpdateNumber) eq '0' or $batch(UpdateNumber) eq '1' ))))");
            });
        }
    }
}
