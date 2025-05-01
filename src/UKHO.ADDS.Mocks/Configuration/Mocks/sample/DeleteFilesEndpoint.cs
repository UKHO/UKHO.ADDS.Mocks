using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.sample
{
    public class DeleteFilesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapDelete("/files", (HttpRequest request) =>
                {
                    return WellKnownStateHandler.HandleWellKnownState(GetState(request));
                })
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Bold("Deletes a file")
                        .AppendNewLine()
                        .Italic("Just a demo method, won't actually delete anything");
                });
        }
    }
}
