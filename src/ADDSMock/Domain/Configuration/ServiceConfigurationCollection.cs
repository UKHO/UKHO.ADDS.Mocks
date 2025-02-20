using System.IO.Abstractions;
using System.Reflection;
using System.Text.Json;
using LightResults;

namespace ADDSMock.Domain.Configuration
{
    public class ServiceConfigurationCollection
    {
        public ServiceConfigurationCollection(IEnumerable<ServiceConfiguration> configurations) => Configurations = new List<ServiceConfiguration>(configurations);

        public IEnumerable<ServiceConfiguration> Configurations { get; }

        public static Result<ServiceConfigurationCollection> ReadConfigurationCollection(IFileSystem fileSystem, string path)
        {
            var configurationPath = fileSystem.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var configurationFilePath = fileSystem.Path.Combine(configurationPath, path);

            if (!fileSystem.Path.Exists(configurationFilePath))
            {
                return Result<ServiceConfigurationCollection>.Fail($"Configuration file not found at {configurationFilePath}");
            }

            try
            {
                var configurationJson = fileSystem.File.ReadAllText(configurationFilePath);
                var configuration = JsonSerializer.Deserialize<ServiceConfigurationCollection>(configurationJson)!;

                return configuration;
            }
            catch (Exception e)
            {
                return Result<ServiceConfigurationCollection>.Fail($"Error reading configuration file: {e.Message}");
            }
        }
    }
}
