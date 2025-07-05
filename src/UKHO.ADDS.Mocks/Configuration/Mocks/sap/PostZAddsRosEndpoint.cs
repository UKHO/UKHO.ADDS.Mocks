using UKHO.ADDS.Mocks.Domain;
using UKHO.ADDS.Mocks.Domain.Markdown;
using UKHO.ADDS.Mocks.Domain.Mocks;
using UKHO.ADDS.Mocks.Domain.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.sap
{
    public class PostZAddsRosEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPost("/z_adds_ros.asmx", (HttpRequest request) =>
            {
                var state = GetState(request);

                switch (state)
                {
                    case WellKnownState.Default:

                        var pathResult = GetFile("response.xml");

                        if (pathResult.IsSuccess(out var file))
                        {
                            return Results.File(file.Open(), file.MimeType);
                        }

                        return Results.NotFound("Could not find response.xml");

                    default:
                        // Just send default responses
                        return WellKnownStateHandler.HandleWellKnownState(state);
                }
            })
            .Produces<string>()
            .WithEndpointMetadata(endpoint, d =>
            {
                d.Append(new MarkdownHeader("SAP Post to z_adds_ros.asmx ", 3));
                d.Append(new MarkdownParagraph("return 200 with the xml body"));
            });
    }
}
