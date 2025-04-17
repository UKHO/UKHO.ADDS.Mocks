using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = "/batch/(.*)/files/(.*)";
    
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader("X-Content-Size", new RegexMatcher(".*")) 
                .WithBody(new JsonMatcher(@"
           {
            ""attributes"": [
              {
                ""key"": ""string"",
                ""value"": ""string""
              }]
           }"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
        );
    
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher("^/batch//files/.*$")) // Matches if batchId is missing
                .UsingPost()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400) // Bad Request
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{ ""error"": ""Batch ID is missing in the URI."" }")
        );
}
