using UKHO.ADDS.Mocks.Markdown;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.sample
{
    public class MultipleMethodsEndpoint : ServiceEndpointMock
    {
        private readonly string[] _methods;

        public MultipleMethodsEndpoint()
        {
            _methods = new[]
            {
                HttpMethod.Get.ToString(),
                HttpMethod.Post.ToString(),
                HttpMethod.Put.ToString(),
                HttpMethod.Delete.ToString(),
                HttpMethod.Head.ToString(),
                HttpMethod.Options.ToString(),
                HttpMethod.Patch.ToString(),
                HttpMethod.Trace.ToString(),
            };
        }

        public override void RegisterSingleEndpoint(IEndpointMock endpoint)
        {
            endpoint.MapMethods("/multiples", _methods, (HttpRequest request) =>
                {
                    var state = GetState(request);

                    return Results.Ok("Something happened");
                })
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Demonstrates multiple HTTP methods", 3));
                    d.Append(new MarkdownParagraph("Just a demo method, nothing too exciting"));
                });
        }
    }
}
