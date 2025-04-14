using System.IO.Abstractions;
using MudBlazor.Services;
using Scalar.AspNetCore;
using Serilog;
using UKHO.ADDS.Mocks.Dashboard;
using UKHO.ADDS.Mocks.Domain.Services;
using UKHO.ADDS.Mocks.Domain.Traffic;

namespace UKHO.ADDS.Mocks
{
    public static class MockServer
    {
        public static async Task RunAsync(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            builder.Services.AddMudServices();
            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi();

            builder.Logging.AddFilter("Microsoft", LogLevel.Critical);

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
    }
}
