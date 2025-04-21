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
                .WithHeader("_X-Correlation-ID", "201-created-guid-fss-add-file")
        );

    server
         .Given(
             Request.Create()
                 .WithPath(urlPattern)
                 .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-add-file")
                 .UsingPost()
         )
         .RespondWith(
             Response.Create()
                 .WithStatusCode(400)
                 .WithHeader("Content-Type", "application/json")
                 .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-add-file")
                 .WithBodyFromFile(mockService.Files.Where(x => x.Name == "add-file-badresponse.json").Select(x => x.Path).FirstOrDefault())
         );

    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .WithHeader("_X-Correlation-ID", "401-unauthorized-guid-fss-add-file")
                .UsingPost()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader("Content-Type", "application/json")
                 .WithHeader("_X-Correlation-ID", "401-unauthorized-guid-fss-add-file")
                .WithBody("Unauthorized")
        );

    server
         .Given(
             Request.Create()
                 .WithPath(urlPattern)
                 .WithHeader("_X-Correlation-ID", "403-forbidden-guid-fss-add-file")
                 .UsingPost()
         )
         .RespondWith(
             Response.Create()
                 .WithStatusCode(403)
                 .WithHeader("Content-Type", "application/json")
                 .WithHeader("_X-Correlation-ID", "403-forbidden-guid-fss-add-file")
                 .WithBody("Forbidden")
         );

    server
         .Given(
             Request.Create()
                 .WithPath(urlPattern)
                 .UsingPost()
                 .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-add-file")
         )
         .RespondWith(
             Response.Create()
                 .WithStatusCode(429)
                 .WithHeader("Content-Type", "application/json")
                 .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-add-file")
                 .WithHeader("Retry-After", "10")
                 .WithBody("Too Many Requests")
         );
}
