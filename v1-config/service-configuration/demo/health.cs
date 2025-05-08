using ADDSMock.Domain.Mappings;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    server
        .Given(
            Request.Create().WithPath("demo/health").UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "text/plain")
                .WithBody("Healthy!"));
}


