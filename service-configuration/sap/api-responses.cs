using ADDSMock.Domain.Mappings;
using System.Net;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    server
         .Given(
             Request.Create()
                 .WithUrl(new RegexMatcher(".*z_adds_ros.asmx.*"))   
                 .UsingGet())
         .RespondWith(
             Response.Create()
                 .WithStatusCode(200)
                 .WithHeader("Content-Type", "application/json")
                 .WithBody("Record successfully received"));

    server
        .Given(
            Request.Create()
                .WithUrl(new RegexMatcher(".*z_adds_mat_info.asmx.*"))    
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("Record successfully received for License GUID"));    
}
