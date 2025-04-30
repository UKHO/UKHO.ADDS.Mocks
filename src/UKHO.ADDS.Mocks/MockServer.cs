using System.IO.Abstractions;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using UKHO.ADDS.Mocks.Dashboard;
using UKHO.ADDS.Mocks.Domain.Internal.Mocks;
using UKHO.ADDS.Mocks.Domain.Internal.Services;
using UKHO.ADDS.Mocks.Domain.Internal.Traffic;
using Radzen;
using UKHO.ADDS.Mocks.Dashboard.Services;

namespace UKHO.ADDS.Mocks
{
    public static class MockServer
    {
        public static async Task RunAsync(string[] args)
        {
            MockServices.AddServices();

            var appContextBase = AppContext.BaseDirectory;

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = appContextBase
            });

            builder.Host.UseSerilog((context, services, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

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
            builder.Services.AddLocalization();

            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi();

            builder.Logging.AddFilter("Microsoft", LogLevel.Critical);

            ConfigureOpenApi(builder);
            ConfigureApplication(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAntiforgery();

            app.MapRazorPages();
            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            app.UseMiddleware<MockTrafficCaptureMiddleware>();

            await StartApplication(app);

            app.MapOpenApi();
            app.MapScalarApiReference(o => o.Servers = []);

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

        private static void ConfigureOpenApi(WebApplicationBuilder builder)
        {
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
}
