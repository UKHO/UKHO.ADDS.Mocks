using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

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
                        try
                        {
                            var fs = GetFileSystem();
                            var s = fs.OpenFile("/response.xml", FileMode.Open, FileAccess.Read);
                            return Results.File(s, MimeType.Application.Xml);
                        }
                        catch (Exception)
                        {
                            return Results.NotFound("Could not find response.xml");
                        }
                    

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
