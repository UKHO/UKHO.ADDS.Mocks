using System.IO.Abstractions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Radzen;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using UKHO.ADDS.Mocks.Api;
using UKHO.ADDS.Mocks.Dashboard;
using UKHO.ADDS.Mocks.Dashboard.Services;
using UKHO.ADDS.Mocks.Domain.Internal.Mocks;
using UKHO.ADDS.Mocks.Domain.Internal.Services;
using UKHO.ADDS.Mocks.Domain.Internal.Traffic;

namespace UKHO.ADDS.Mocks
{
    public static class MockServer
    {
        public static async Task RunAsync(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            MockServices.AddServices();

            var appContextBase = AppContext.BaseDirectory;

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = args, ContentRootPath = appContextBase });

            var oltpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!;

            builder.Services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.OpenTelemetry(o =>
                {
                    o.Endpoint = oltpEndpoint;
                })
                .WriteTo.Console()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .MinimumLevel.Override("Azure.Core", LogEventLevel.Fatal)
                .MinimumLevel.Override("Azure.Storage.Blobs", LogEventLevel.Fatal)
                .MinimumLevel.Override("Azure.Storage.Queues", LogEventLevel.Warning));

            builder.WebHost.UseStaticWebAssets();

            builder.Services.AddRazorPages();
            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            builder.Services.AddSingleton(sp =>
            {
                // Get the address that the app is currently running at
                var server = sp.GetRequiredService<IServer>();
                var addressFeature = server.Features.Get<IServerAddressesFeature>();
                var baseAddress = addressFeature != null ? addressFeature.Addresses.First() : string.Empty;
                return new HttpClient { BaseAddress = new Uri(baseAddress) };
            });


            builder.Services.AddRadzenComponents();
            builder.Services.AddRadzenQueryStringThemeService();

            builder.Services.AddScoped<DashboardPageService>();
            builder.Services.AddSingleton<DashboardService>();
            builder.Services.AddLocalization();

            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi();

            builder.Logging.AddFilter("Microsoft", LogLevel.Critical);

            ConfigureOpenApi(builder);
            ConfigureApplication(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAntiforgery();

            app.MapRazorPages();
            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            app.UseMiddleware<MockTrafficCaptureMiddleware>();

            await StartApplication(app);

            app.MapOpenApi();
            app.MapScalarApiReference(o => o.Servers = []);

            app.RegisterAdminApi();

            await app.RunAsync();
        }

        private static async Task StartApplication(WebApplication app)
        {
            var mappingService = app.Services.GetRequiredService<MappingService>();

            await mappingService.BuildDefinitionsAsync(app.Lifetime.ApplicationStopping);
            await mappingService.ApplyDefinitionsAsync(app, app.Lifetime.ApplicationStopping);
        }

        private static void ConfigureApplication(WebApplicationBuilder builder)
        {
            var fileSystem = new FileSystem();

            builder.Services.AddSingleton<IFileSystem>(x => fileSystem);
            builder.Services.AddSingleton(x => new EnvironmentService(builder.Environment));

            builder.Services.AddSingleton<MappingService>();
        }

        private static void ConfigureOpenApi(WebApplicationBuilder builder) =>
            builder.Services.AddOpenApi(options =>
            {
                options.AddOperationTransformer((operation, context, cancellationToken) =>
                {
                    var headers = context.Description.ActionDescriptor.EndpointMetadata
                        .OfType<OpenApiHeaderParameter>();

                    foreach (var header in headers)
                    {
                        operation.Parameters ??= new List<OpenApiParameter>();

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = header.Name,
                            In = ParameterLocation.Header,
                            Required = header.Required,
                            Description = header.Description,
                            Schema = new OpenApiSchema
                            {
                                Type = "string",
                                Enum = header.ExpectedValues?
                                    .Select(v => new OpenApiString(v))
                                    .Cast<IOpenApiAny>()
                                    .ToList()
                            }
                        });
                    }

                    return Task.CompletedTask;
                });
            });
    }
}
