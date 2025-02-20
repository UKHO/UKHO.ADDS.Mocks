using System.IO.Abstractions;
using ADDSMock.Applications.Interactive.Logging;
using ADDSMock.Applications.Interactive.Services;
using ADDSMock.Applications.Interactive.Services.Implementation;
using ADDSMock.Applications.Interactive.Views.Explorer;
using ADDSMock.Applications.Interactive.Views.Mappings;
using ADDSMock.Applications.Interactive.Views.Traffic;
using ADDSMock.Applications.Interactive.Windows;
using ADDSMock.Domain.Configuration;
using ADDSMock.Domain.Services;
using ADDSMock.Domain.Services.Runtime;
using LightResults;
using Serilog;

namespace ADDSMock.Extensions
{
    internal static class InjectionExtensions
    {
        public static Result AddConsole(this IServiceCollection collection)
        {
            var logConfiguration = new LoggerConfiguration();
            logConfiguration.WriteTo.Console();

            Log.Logger = logConfiguration.CreateLogger();

            var commonResult = AddCommon(collection, false);

            if (commonResult.IsFailed)
            {
                return Result.Fail(commonResult.Error);
            }

            collection.AddSingleton<ILoggingService, LoggingService>();

            return Result.Ok();
        }

        public static Result AddInteractive(this IServiceCollection collection)
        {
            var logConfiguration = new LoggerConfiguration();
            var consoleLog = new ConsoleLog();

            logConfiguration.WriteTo.InteractiveConsole(consoleLog);

            Log.Logger = logConfiguration.CreateLogger();

            var commonResult = AddCommon(collection, true);

            if (commonResult.IsFailed)
            {
                return Result.Fail(commonResult.Error);
            }

            collection.AddSingleton<ILoggingService, InteractiveLoggingService>();

            collection.AddSingleton<IViewService, ViewService>();
            collection.AddSingleton(x => consoleLog);

            collection.AddSingleton<FrameWindow>();
            collection.AddSingleton<FrameWindowModel>();

            collection.AddSingleton<ExplorerPaneModel>();
            collection.AddSingleton<ExplorerPaneView>();
            collection.AddSingleton<MappingsPaneModel>();
            collection.AddSingleton<MappingsPaneView>();
            collection.AddSingleton<TrafficPaneModel>();
            collection.AddSingleton<TrafficPaneView>();

            collection.AddSingleton<ExplorerModel>();
            collection.AddSingleton<ExplorerView>();
            collection.AddSingleton<MappingsModel>();
            collection.AddSingleton<MappingsView>();
            collection.AddSingleton<TrafficModel>();
            collection.AddSingleton<TrafficView>();

            return AddCommon(collection, true);
        }

        private static Result AddCommon(IServiceCollection collection, bool isInteractive)
        {
            var fileSystem = new FileSystem();

            var mockConfigurationResult = MockConfiguration.ReadConfiguration(fileSystem, "mock-configuration.json");

            if (mockConfigurationResult.IsFailed)
            {
                Log.Error(mockConfigurationResult.Error.Message);
                return Result.Fail(mockConfigurationResult.Error);
            }

            var serviceConfigurationResult = ServiceConfigurationCollection.ReadConfigurationCollection(fileSystem, "service-configuration.json");

            if (serviceConfigurationResult.IsFailed)
            {
                Log.Error(serviceConfigurationResult.Error.Message);
                return Result.Fail(serviceConfigurationResult.Error);
            }

            collection.AddSingleton<IFileSystem>(x => fileSystem);
            collection.AddSingleton(mockConfigurationResult.Value);
            collection.AddSingleton(serviceConfigurationResult.Value);

            collection.AddSingleton<IWireMockService, WireMockService>();

            collection.AddSingleton<IEnvironmentService>(x =>
                new EnvironmentService(x.GetRequiredService<MockConfiguration>(),
                    x.GetRequiredService<ServiceConfigurationCollection>(),
                    isInteractive));

            collection.AddSingleton<IMappingService, MappingService>();

            return Result.Ok();
        }
    }
}
