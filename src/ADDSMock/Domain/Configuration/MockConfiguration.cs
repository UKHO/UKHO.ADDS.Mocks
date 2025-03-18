using System.Diagnostics;
using System.IO.Abstractions;
using System.Reflection;
using System.Text.Json;
using CSScripting;
using LightResults;

namespace ADDSMock.Domain.Configuration
{
    public class MockConfiguration
    {
        public MockConfiguration(string configurationPath, string overrideConfigurationPath, int port, bool useSsl, bool useHttp2)
        {
            ConfigurationPath = configurationPath;
            OverrideConfigurationPath = overrideConfigurationPath;
            Port = port;
            UseSsl = useSsl;
            UseHttp2 = useHttp2;
        }

        public string ConfigurationPath { get; set; }
        public string OverrideConfigurationPath { get; set; }

        public int Port { get; }
        public bool UseSsl { get; }
        public bool UseHttp2 { get; }

        public static Result<MockConfiguration> ReadConfiguration(IFileSystem fileSystem, string path)
        {
            var configurationPath = fileSystem.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var currentDirectoryPath = fileSystem.Directory.GetCurrentDirectory();

            var solutionDirectoryPath = fileSystem.Path.Combine(currentDirectoryPath, @"..\..\");
            var configurationDirectoryPath = fileSystem.Path.GetFullPath(solutionDirectoryPath);

            var serviceConfigurationPath = fileSystem.Path.Combine(configurationPath, "service-configuration");
            var overrideConfigurationPath = fileSystem.Path.Combine(configurationPath, "override-configuration");

            var configurationFilePath = fileSystem.Path.Combine(configurationPath, path);

            if (!fileSystem.Path.Exists(configurationFilePath))
            {
                return Result<MockConfiguration>.Fail($"Configuration file not found at {configurationFilePath}");
            }

            try
            {
                var configurationJson = fileSystem.File.ReadAllText(configurationFilePath);
                var configuration = JsonSerializer.Deserialize<MockConfiguration>(configurationJson)!;
                // This code will only run in Debug mode
#if DEBUG
                
                    configuration.ConfigurationPath = serviceConfigurationPath;
                    configuration.OverrideConfigurationPath = overrideConfigurationPath;
                
#endif

                return configuration;
            }
            catch (Exception e)
            {
                return Result<MockConfiguration>.Fail($"Error reading configuration file: {e.Message}");
            }
        }
    }
}
