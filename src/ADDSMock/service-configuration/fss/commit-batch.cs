using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using System.Text.RegularExpressions;
using WireMock.Http;
using System.Linq;
using WireMock.Types;
using System;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch/(.*)";

    // 202 Accepted Response with Commit Batch
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(@"/batch/[a-fA-F0-9-]{36}")) 
                .UsingPut() 
                .WithBody("[{\"filename\":\"string\",\"hash\":\"string\"}]") 
        )
        .RespondWith(
            Response.Create()
            .WithHeader("X-Correlation-ID", "202-Accepted-guid-fss-commit-batch")
                .WithCallback(request =>
                {
                    var batchId = request.PathSegments.ElementAtOrDefault(2); 
                    return new WireMock.ResponseMessage
                    {
                        StatusCode = 202, 
                        Headers = new Dictionary<string, WireMockList<string>>
                        {
                            { "Content-Type", new WireMockList<string>("application/json") }
                        },
                        BodyData = new WireMock.Util.BodyData
                        {
                            BodyAsJson = new
                            {
                                status = new
                                {
                                    uri = $"/batch/{batchId}/status" 
                                }
                            },
                            DetectedBodyType = BodyType.Json 
                        }
                    };
                })
        );

    // 400 Bad Request Response
    server
        .Given(
            Request.Create()
                .WithPath(new RegexMatcher(@"/batch/[a-fA-F0-9-]{36}"))
                .WithHeader("X-Correlation-ID", "400-badrequest-guid-fss-commit-batch")
                .UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithHeader("X-Correlation-ID", "400-badrequest-guid-fss-commit-batch")
                .WithBodyAsJson(new
                {
                    correlationId = "400-badrequest-guid-fss-commit-batch",
                    errors = new[]
                    {
                        new
                        {
                            source = "Commit Batch",
                            description = "Invalid batchId."
                        }
                    }
                })
        );
}
