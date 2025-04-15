using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch";

    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithBody(new JsonMatcher(@"
                {
                  ""businessUnit"": ""string"",
                  ""acl"": {
                    ""readUsers"": [""string""],
                    ""readGroups"": [""public""]
                  },
                  ""attributes"": [
                    {
                      ""key"": ""string"",
                      ""value"": ""string""
                    }
                  ],
                  ""expiryDate"": ""2025-02-10T11:25:04.982Z""
                }"))
                .WithBody(new RegexMatcher(@".*"))
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new
                {
                    batchId = Guid.NewGuid().ToString()
                })
        );

    server
     .Given(
         Request.Create()
             .WithPath(urlPattern)
             .UsingPost()
             .WithBody(new RegexMatcher(@"""expiryDate"":\s*(null|""(?!\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?Z).*"")"))
     )
     .RespondWith(
         Response.Create()
             .WithStatusCode(400)
             .WithHeader("Content-Type", "application/json")
             .WithBody("Invalid Expiry Date Format.")
     );
}
