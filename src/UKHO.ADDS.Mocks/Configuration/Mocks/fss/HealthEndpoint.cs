using UKHO.ADDS.Mocks.Domain;
using UKHO.ADDS.Mocks.Domain.Markdown;
using UKHO.ADDS.Mocks.Domain.Mocks;
using UKHO.ADDS.Mocks.Domain.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.fss
{
    public class HealthEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapGet("/health", (HttpRequest request) =>
            {
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:

                        return Results.Ok("Healthy!");

                    default:
                        // Just send default responses
                        return WellKnownStateHandler.HandleWellKnownState(state);
                }
            })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Return Healthy", 3));
                    d.Append(new MarkdownParagraph("Just returns a 200"));
                });
    }

}
