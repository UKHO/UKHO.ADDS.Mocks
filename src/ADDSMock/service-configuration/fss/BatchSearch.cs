using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Http;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;
using WireMock.ResponseProviders;
using WireMock.Settings;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System;
using ADDSMock.Provider;

public void RegisterFragment(WireMockServer server, MockService mockService)
{
    var urlPattern = ".*/batch.*";
    server
        .Given(
            Request.Create()
                .WithPath(urlPattern)
                .WithParam("limit", "100")
                .WithParam("start", "0")
                .WithParam("$filter", "*")
                .UsingGet()
        )

    .RespondWith(
        Response.Create()
            .WithCallback(request =>
            {
                var filter = request.Query["$filter"].FirstOrDefault();
                return SearchFilterProvider.ProvideSearchFilterResponse(request);
            })
    );
}




