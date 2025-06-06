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
                .WithParam("$filter", "fields/Title eq '1'")   //200 OK response with 5 UPNs
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($@"
                   {{
                    ""value"": [
                        {{
                            ""fields"": {{
                                ""Title"": ""1"",
                                ""ECDIS_UPN1_Title"": ""Master"",
                                ""ECDIS_UPN_1"": ""C23DAD797C966EC9F6A55B66ED98281599B3A231859868A"",
                                ""ECDIS_UPN2_Title"": ""Backup 1"",
                                ""ECDIS_UPN_2"": ""BA2DAD797C966EC9F6A55B66ED98281599B3C7B1859868B"",
                                ""ECDIS_UPN3_Title"": ""Backup 2"",
                                ""ECDIS_UPN_3"": ""A39E2BD79F867CA1B52A44C16FD98281599FA2C31595878"",
                                ""ECDIS_UPN4_Title"": ""Backup 3"",
                                ""ECDIS_UPN_4"": ""D43BFA197C562EB8F73A23D45ED98270599B1C54784968D"",
                                ""ECDIS_UPN5_Title"": ""Backup 4"",
                                ""ECDIS_UPN_5"": ""E18ACD947B726FC3A85C66B19AD98231589D3A42765987A""
                            }}
                        }}
                        ]
                    }}"
                ));

    server
        .Given(
            Request.Create()
                .WithParam("$filter", "fields/Title eq '2'")    //500 internal server error response
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(500)
                .WithHeader("Content-Type", "application/json"));

    server
        .Given(
            Request.Create()
                .WithParam("$filter", "fields/Title eq '3'")    //200 OK response with 0 UPNs to generate 404 response in Shop Facade
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($@"
                    {{
                        ""value"": []
                    }}"
                ));
    server
        .Given(
            Request.Create()
                .WithParam("$filter", "fields/Title eq '4'")    //200 OK response with 0 UPNs and some metadata to generate 204 response in Shop Facade
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($@"
                    {{
                        ""value"": [
                            {{
                                ""fields"": {{
                                    ""@odata.etag"": ""975454d2-ead9-482e-8472-7c620847fae8""                                    
                                }}
                            }}
                            ]                        
                    }}"
                ));

    server
        .Given(
            Request.Create()
                .WithParam("$filter", "fields/Title eq '5'")    //401 Authorized response
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithHeader("Content-Type", "application/json"));

    server
        .Given(
            Request.Create()
                .WithParam("$filter", "fields/Title eq '6'")    //403 Forbidden response
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(403)
                .WithHeader("Content-Type", "application/json"));

    server
        .Given(
            Request.Create()
                .WithParam("$filter", "fields/Title eq '7'")   //200 OK response with keyword "400BadRequestResponse" in UPN to generate 400 response from PGS
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($@"
                   {{
                    ""value"": [
                        {{
                            ""fields"": {{
                                ""Title"": ""1"",
                                ""ECDIS_UPN1_Title"": ""Master"",
                                ""ECDIS_UPN_1"": ""400BadRequestResponseB66ED98281599B3A231859868A""
                            }}
                        }}
                        ]
                    }}"
                ));

    server
        .Given(
            Request.Create()
                .WithParam("$filter", "fields/Title eq '8'")   //200 OK response with keyword "500InternalServerErrorResponse" in UPN to generate 500 response from PGS
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($@"
                   {{
                    ""value"": [
                        {{
                            ""fields"": {{
                                ""Title"": ""1"",
                                ""ECDIS_UPN1_Title"": ""Master"",
                                ""ECDIS_UPN_1"": ""500InternalServerErrorResponse1599B3A231859868A""
                            }}
                        }}
                        ]
                    }}"
                ));

    server
        .Given(
            Request.Create()
                .WithParam("$filter", "fields/Title eq '9'")   //200 OK response with keyword "401UnauthorizedResponse" in UPN to generate 401 response from PGS
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($@"
                   {{
                    ""value"": [
                        {{
                            ""fields"": {{
                                ""Title"": ""1"",
                                ""ECDIS_UPN1_Title"": ""Master"",
                                ""ECDIS_UPN_1"": ""401UnauthorizedResponse6ED98281599B3C7B1859868B""
                            }}
                        }}
                        ]
                    }}"
                ));

    server
        .Given(
            Request.Create()
                .WithParam("$filter", "fields/Title eq '10'")   //200 OK response with keyword "403ForbiddenResponse" in UPN to generate 403 response from PGS
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($@"
                   {{
                    ""value"": [
                        {{
                            ""fields"": {{
                                ""Title"": ""1"",
                                ""ECDIS_UPN1_Title"": ""Master"",
                                ""ECDIS_UPN_1"": ""403ForbiddenResponse5B66ED98281599B3A231859868A""
                            }}
                        }}
                        ]
                    }}"
                ));

    server
        .Given(
            Request.Create()
                .WithParam("$filter", "fields/Title eq '11'")   //200 OK response with keyword "404NotFoundResponse" in UPN to generate 404 response from PGS
                .UsingGet())
        .RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($@"
                   {{
                    ""value"": [
                        {{
                            ""fields"": {{
                                ""Title"": ""1"",
                                ""ECDIS_UPN1_Title"": ""Master"",
                                ""ECDIS_UPN_1"": ""404NotFoundResponse35B66ED98281599B3A231859868A""
                            }}
                        }}
                        ]
                    }}"
                ));
}
