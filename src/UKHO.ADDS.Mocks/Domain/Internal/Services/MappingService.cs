using System.IO.Abstractions;
using System.Reflection;
using UKHO.ADDS.Mocks.Configuration;

namespace UKHO.ADDS.Mocks.Domain.Internal.Services
{
    internal class MappingService
    {
        private readonly IFileSystem _fileSystem;

        public MappingService(IFileSystem fileSystem) => _fileSystem = fileSystem;

        public Task ApplyDefinitionsAsync(WebApplication app, CancellationToken cancellationToken)
        {
            var correctDefinitions = ServiceRegistry.Definitions.Where(x => !x.HasError).ToList();

            foreach (var definition in correctDefinitions)
            {
                var group = app.MapGroup(definition.Prefix);

                try
                {
                    foreach (var serviceFragment in definition.ServiceFragments)
                    {
                        var serviceMock = (ServiceEndpointMock)Activator.CreateInstance(serviceFragment.Type)!;

                        serviceMock.SetDefinition(definition);
                        serviceMock.RegisterSingleEndpoint(serviceFragment.CreateBuilder(group));
                    }
                }
                catch (Exception ex)
                {
                    // Set into definition
                    definition.SetError($"Error while executing definition: {ex.Message}");
                }
            }

            return Task.CompletedTask;
        }

        public Task BuildDefinitionsAsync(CancellationToken cancellationToken)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var allTypes = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(ServiceEndpointMock).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
                .ToList();

            var exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

            foreach (var definition in ServiceRegistry.Definitions)
            {
                try
                {
                    var coreCodeNamespace = $"UKHO.ADDS.Mocks.Configuration.Mocks.{definition.Prefix}";
                    var overrideCodeNamespace = $".Override.Mocks.{definition.Prefix}";

                    var coreFilePath = Path.Combine(exeDirectory, $"Configuration/Files/{definition.Prefix}");
                    var overrideFilePath = Path.Combine(exeDirectory, $"Override/Files/{definition.Prefix}");

                    var coreServiceMocks = allTypes.Where(x => x.Namespace!.Equals(coreCodeNamespace, StringComparison.InvariantCultureIgnoreCase)).Select(t => t).ToList();
                    var overrideServiceMocks = allTypes.Where(x => !x.Namespace!.Equals(coreCodeNamespace, StringComparison.InvariantCultureIgnoreCase) &&
                                                                   x.Namespace.Contains(overrideCodeNamespace, StringComparison.InvariantCultureIgnoreCase)).Select(t => t).ToList();

                    var serviceMockTypes = new Dictionary<string, (Type, bool)>();

                    foreach (var type in coreServiceMocks)
                    {
                        serviceMockTypes[type.Name] = (type, false);
                    }

                    foreach (var type in overrideServiceMocks)
                    {
                        serviceMockTypes[type.Name] = (type, true);
                    }

                    var serviceFiles = CreateFileList(coreFilePath, overrideFilePath, "*.*");

                    definition.AddServiceMockTypes(serviceMockTypes);
                    definition.AddFilePaths(serviceFiles);
                }
                catch (Exception ex)
                {
                    // Set into definition
                    definition.SetError($"Error during mapping: {ex.Message}");
                }
            }

            return Task.CompletedTask;
        }

        private IDictionary<string, (string, bool)> CreateFileList(string corePath, string overridePath, string queryPattern)
        {
            var fileDictionary = new Dictionary<string, (string, bool)>();

            if (_fileSystem.Path.Exists(corePath))
            {
                var coreFiles = _fileSystem.Directory.GetFiles(corePath, queryPattern, SearchOption.AllDirectories);

                foreach (var filePath in coreFiles)
                {
                    var name = _fileSystem.Path.GetFileName(filePath);
                    fileDictionary.Add(name, (filePath, false));
                }
            }

            if (_fileSystem.Path.Exists(overridePath))
            {
                var overrideFiles = _fileSystem.Directory.GetFiles(overridePath, queryPattern, SearchOption.AllDirectories);

                foreach (var filePath in overrideFiles)
                {
                    var name = _fileSystem.Path.GetFileName(filePath);
                    fileDictionary[name] = (filePath, true);
                }
            }

            return fileDictionary;
        }
    }
}
