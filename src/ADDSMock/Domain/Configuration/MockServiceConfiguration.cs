using System.IO.Abstractions;
using System.Reflection;
using System.Text.Json;
using LightResults;

namespace ADDSMock.Domain.Configuration
{
    public class MockServiceConfiguration
    {
        public MockServiceConfiguration(IReadOnlyDictionary<string, string> items) => Items = items;


        public IReadOnlyDictionary<string, string> Items { get; }

        public static Result<MockServiceConfiguration> ReadConfiguration(IFileSystem fileSystem, string path)
        {
            var configurationPath = fileSystem.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var configurationFilePath = fileSystem.Path.Combine(configurationPath, path);

            if (!fileSystem.Path.Exists(configurationFilePath))
            {
                return Result<MockServiceConfiguration>.Fail($"Configuration file not found at {configurationFilePath}");
            }

            try
            {
                var configurationJson = fileSystem.File.ReadAllText(configurationFilePath);
                var configuration = JsonSerializer.Deserialize<MockServiceConfiguration>(configurationJson)!;

                return configuration;
            }
            catch (Exception e)
            {
                return Result<MockServiceConfiguration>.Fail($"Error reading configuration file: {e.Message}");
            }
        }
    }
}
