using System.ComponentModel;
using System.IO.Abstractions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using Scalar.AspNetCore;
using Serilog;
using UKHO.ADDS.Mocks.Dashboard;
using UKHO.ADDS.Mocks.Domain.Internal.Mocks;
using UKHO.ADDS.Mocks.Domain.Internal.Services;
using UKHO.ADDS.Mocks.Domain.Internal.Traffic;

namespace UKHO.ADDS.Mocks
{
    public static class MockServer
    {
        public static async Task RunAsync(string[] args)
        {
            MockServices.AddServices();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            builder.Services.AddMudServices();
            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi();

            builder.Logging.AddFilter("Microsoft", LogLevel.Critical);

            ConfigureOpenApi(builder);
            ConfigureApplication(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseMiddleware<MockTrafficCaptureMiddleware>();

            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseAntiforgery();
            app.UseStaticFiles();

            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            ConfigureCaching(app);
            await StartApplication(app);

            app.MapOpenApi();
            app.MapScalarApiReference(o => o.Servers = []);

            await app.RunAsync();
        }

        private static void ConfigureCaching(WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
                context.Response.Headers.Pragma = "no-cache";
                context.Response.Headers.Expires = "0";
                await next();
            });
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
