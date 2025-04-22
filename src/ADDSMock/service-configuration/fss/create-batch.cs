using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch";

    // 201 Created Response
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
                .WithHeader("_X-Correlation-ID", "201-created-guid-fss-create-batch")
                .WithBodyAsJson(new
                {
                    batchId = Guid.NewGuid().ToString()
                })
        );

    // 400 Bad Request Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-create-batch")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "400-badrequest-guid-fss-create-batch")
                .WithBodyAsJson(new
                {
                    correlationId = "400-badrequest-guid-fss-create-batch",
                    errors = new[]
                    {
                        new
                        {
                            source = "Create Batch",
                            description = "Invalid Expiry Date Format."
                        }
                    }
                })
        );

    // 401 Unauthorized Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader("_X-Correlation-ID", "401-unauthorised-guid-fss-create-batch")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "401-unauthorised-guid-fss-create-batch")
                .WithBody("Unauthorised.")
        );

    // 403 Forbidden Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-fss-create-batch")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "403-forbidden-guid-fss-create-batch")
                .WithBody("Forbidden.")
        );

    // 429 Too Many Requests Response
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .UsingPost()
                .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-create-batch")
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(429)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("_X-Correlation-ID", "429-toomanyrequests-guid-fss-create-batch")
                .WithHeader("Retry-After", "10")
                .WithBody("Too Many Requests.")
        );
}
