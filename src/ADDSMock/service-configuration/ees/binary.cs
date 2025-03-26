using ADDSMock.Domain.Mappings;
using System.Net;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    server
        .Given(
            Request.Create().WithPath("erp/some/thing").UsingGet()
        )
        .RespondWith(
            Response.Create()
                .WithBody(new byte[] { 48, 0x65, 0x6c, 0x6c, 0x6f })
        );
}
