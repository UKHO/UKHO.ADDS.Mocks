using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.scs
{
    public class GetBasicCatalogueEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapGet("/v2/catalogues/{productType}/basic", (string productType, HttpRequest request) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:

                            switch (productType.ToLowerInvariant())
                            {
                                case "s100":
                                    var pathResult = endpoint.GetFile("s100-catalogue.json");

                                    if (pathResult.IsSuccess(out var file))
                                    {
                                        return Results.File(file.Path, contentType: MimeType.Application.Json);
                                    }

                                    return Results.NotFound("Could not find the path in the /files GET method");
                                default:
                                    return Results.NotFound();
                            }

                        default:
                            // Just send default responses
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Gets a basic catalog", 3));
                    d.Append(new MarkdownParagraph(new MarkdownEmphasis("Only s100 is implemented at the moment")));
                });
        }
    }
}
