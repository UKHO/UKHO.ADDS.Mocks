using ADDSMock.Domain.Mappings;
using System.Net;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    server
        .Given(
            Request.Create().WithPath("sap/health").UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "text/plain")
                .WithBody("Healthy!"));
}
