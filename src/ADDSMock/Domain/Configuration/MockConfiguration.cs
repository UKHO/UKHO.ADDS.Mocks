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
            var currentDiretory = Directory.GetCurrentDirectory(); 
            var solutionfolder = Path.Combine(currentDiretory, @"..\..\");            ;
            var configurationFolderPath = Path.GetFullPath(solutionfolder);
            var servieConfigurationPath = Path.Combine(configurationFolderPath, "service-configuration");
            var overrideConfigurationPath = Path.Combine(configurationFolderPath, "override-configuration");

            var configurationFilePath = fileSystem.Path.Combine(configurationPath, path);

            if (!fileSystem.Path.Exists(configurationFilePath))
            {
                return Result<MockConfiguration>.Fail($"Configuration file not found at {configurationFilePath}");
            }

            try
            {
                var configurationJson = fileSystem.File.ReadAllText(configurationFilePath);
                var configuration = JsonSerializer.Deserialize<MockConfiguration>(configurationJson)!;
                configuration.ConfigurationPath = servieConfigurationPath;
                configuration.OverrideConfigurationPath = overrideConfigurationPath;

                return configuration;
            }
            catch (Exception e)
            {
                return Result<MockConfiguration>.Fail($"Error reading configuration file: {e.Message}");
            }
        }
    }
}
