using System.IO.Abstractions;
using System.Reflection;
using System.Text;
using ADDSMock.Domain.Compiler;
using ADDSMock.Domain.Configuration;
using ADDSMock.Domain.Events;
using ADDSMock.Domain.Mappings;
using CSScriptLib;
using LightResults;
using ReactiveUI;
using Serilog;
using WireMock.Matchers.Request;
using WireMock.Server;

// ReSharper disable UsageOfDefaultStructEquality

namespace ADDSMock.Domain.Services.Runtime
{
    internal class MappingService : IMappingService
    {
        private readonly IEnvironmentService _environmentService;
        private readonly IFileSystem _fileSystem;

        private readonly string _fragmentCodeTemplate;

        private List<MockService> _services;

        private readonly string _defaultNamespaces;
        //private readonly ScriptParser _scriptParser;

        public MappingService(IEnvironmentService environmentService, IFileSystem fileSystem)
        {
            _environmentService = environmentService;
            _fileSystem = fileSystem;
        
            _services = [];

            _fragmentCodeTemplate = ReadFragmentCodeTemplate();

            CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Roslyn;

            var evaluator = CSScript.Evaluator;
            evaluator.ReferenceAssemblyOf<WireMockServer>();
            evaluator.ReferenceAssemblyOf<IRequestMatcher>();

            string[] namespaces =
            [
                "System", "System.Text", "System.Reflection", "System.IO", "System.Net", "System.Net.Http",
                "System.Collections", "System.Collections.Generic", "System.Collections.Concurrent",
                "System.Text.RegularExpressions", "System.Threading.Tasks", "System.Linq",
                "WireMock.RequestBuilders", "WireMock.ResponseBuilders", "WireMock.Server", "WireMock.Settings",
                "ADDSMock.Domain.Compiler", "ADDSMock.Domain.Mappings", "ADDSMock.Domain.Configuration"
            ];

            foreach (var ns in namespaces)
            {
                evaluator.ReferenceAssemblyByNamespace(ns);
            }

            var usingBuilder = new StringBuilder();

            foreach (var ns in namespaces)
            {
                usingBuilder.AppendLine($"using {ns};");
            }

            usingBuilder.AppendLine();

            _defaultNamespaces = usingBuilder.ToString();

            //_scriptParser = new ScriptParser();
            //_scriptParser.ScriptEngine.AddAssembly(typeof(WireMockServer));
            //_scriptParser.ScriptEngine.AddAssembly(typeof(IRequestMatcher));

            //_scriptParser.ScriptEngine.AddNamespace("ADDSMock.Domain.Mappings");
        }

        public IEnumerable<MockService> Services => _services;

        public async Task ReloadMappingsAsync(IWireMockService wireMockService)
        {
            MessageBus.Current.SendMessage(new MappingsReloadingEvent());

            _services.Clear();
            await ReadMappingsAsync();
            await ExecuteMappingsAsync(wireMockService);
        }

        public async Task<Result> ReadMappingsAsync()
        {
            _services = _environmentService.Services.Configurations.Select(x => new MockService(x.Prefix, x.Name)).ToList();

            foreach (var service in _services)
            {
                var servicePath = _fileSystem.Path.Combine(_environmentService.Mock.ConfigurationPath.Replace(@"..\..\",""), service.ServicePrefix);
                var overrideServicePath = _fileSystem.Path.Combine(_environmentService.Mock.OverrideConfigurationPath, service.ServicePrefix);

                var serviceConfigPath = _fileSystem.Path.Combine(servicePath, "configuration.json");
                var overrideServiceConfigPath = _fileSystem.Path.Combine(overrideServicePath, "configuration.json");

                var filesPath = _fileSystem.Path.Combine(servicePath, "files");
                var overrideFilesPath = _fileSystem.Path.Combine(overrideServicePath, "files");

                Console.WriteLine($"Service path: {servicePath}");
                if (!_fileSystem.Path.Exists(servicePath))
                {
                    return Result.Fail($"Service path not found at {servicePath}");
                }

                if (!_fileSystem.Path.Exists(serviceConfigPath))
                {
                    return Result.Fail($"Service configuration file not found at {serviceConfigPath}");
                }

                var serviceConfigurationResult = MockServiceConfiguration.ReadConfiguration(_fileSystem, serviceConfigPath);

                if (serviceConfigurationResult.IsFailed)
                {
                    return Result.Fail(serviceConfigurationResult.Error);
                }

                var configuration = new Dictionary<string, string>(serviceConfigurationResult.Value.Items);
                var fragments = _fileSystem.Directory.EnumerateFiles(servicePath, "*.cs");

                var fragmentDictionary = new Dictionary<string, (string content, bool isOverride)>();

                foreach (var fragment in fragments)
                {
                    var fragmentCode = await _fileSystem.File.ReadAllTextAsync(fragment);
                    var fragmentName = _fileSystem.Path.GetFileNameWithoutExtension(fragment);

                    fragmentDictionary.Add(fragmentName, (fragmentCode, false));
                }

                var serviceFiles = new Dictionary<string, (string name, string mime, bool isOverride)>();

                if (_fileSystem.Directory.Exists(filesPath))
                {
                    var files = _fileSystem.Directory.EnumerateFiles(filesPath, "*.*");

                    foreach (var file in files)
                    {
                        var fileName = _fileSystem.Path.GetFileName(file);
                        serviceFiles[fileName] = (file, MimeTypes.Lookup(file), false);
                    }
                }

                if (_fileSystem.Directory.Exists(overrideServicePath))
                {
                    var overrideFragments = _fileSystem.Directory.EnumerateFiles(overrideServicePath, "*.cs");

                    foreach (var fragment in overrideFragments)
                    {
                        var fragmentCode = await _fileSystem.File.ReadAllTextAsync(fragment);
                        var fragmentName = _fileSystem.Path.GetFileNameWithoutExtension(fragment);

                        fragmentDictionary[fragmentName] = (fragmentCode, true);
                    }

                    if (_fileSystem.File.Exists(overrideServiceConfigPath))
                    {
                        var overrideServiceConfigurationResult = MockServiceConfiguration.ReadConfiguration(_fileSystem, overrideServiceConfigPath);

                        if (overrideServiceConfigurationResult.IsSuccess)
                        {
                            var overrideConfiguration = new Dictionary<string, string>(overrideServiceConfigurationResult.Value.Items);

                            foreach (var (key, value) in overrideConfiguration)
                            {
                                configuration[key] = value;
                            }
                        }
                    }

                    if (_fileSystem.Directory.Exists(overrideFilesPath))
                    {
                        var files = _fileSystem.Directory.EnumerateFiles(overrideFilesPath, "*.*");

                        foreach (var file in files)
                        {
                            var fileName = _fileSystem.Path.GetFileName(file);
                            serviceFiles[fileName] = (file, MimeTypes.Lookup(file), true);
                        }
                    }
                }
                else
                {
                    Log.Warning($"Service configuration overrides not found at {overrideServicePath}");
                }

                service.AddConfiguration(new MockServiceConfiguration(configuration));
                service.AddFragments(fragmentDictionary.Select(x => new MockServiceFragment(x.Key, x.Value.content, x.Value.isOverride)));
                service.AddFiles(serviceFiles.Select(x => new MockServiceFile(x.Key, x.Value.name, x.Value.mime, x.Value.isOverride)));
            }

            return Result.Ok();
        }

        public async Task<Result> ExecuteMappingsAsync(IWireMockService wireMockService)
        {
            foreach (var service in _services)
            {
                foreach (var fragment in service.Fragments)
                {
                    if (service.ServicePrefix == "scs")
                    {
                        var zz = 42;
                    }

                    var beforeConfigurationIds = wireMockService.WireMockServer!.MappingModels.Select(x => x.Guid).ToList();

                    try
                    {
                        var scriptCode = _defaultNamespaces + fragment.Content;

                        dynamic script = CSScript.Evaluator.LoadMethod(scriptCode);

                        script.RegisterFragment(wireMockService.WireMockServer!, service);

                        Log.Information($"Code mapping fragment {service.ServicePrefix}/{fragment.FragmentName} executed");
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Code mapping fragment {service.ServicePrefix}/{fragment.FragmentName} failed: {e.Message}");
                        fragment.SetError(e.Message);
                    }

                    var afterConfiguration = wireMockService.WireMockServer!.MappingModels.ToList();
                    var addedMappings = afterConfiguration.Where(x => !beforeConfigurationIds.Contains(x.Guid)).ToList();

                    fragment.AddMappings(addedMappings);
                }
            }

            MessageBus.Current.SendMessage(new MockServicesUpdatedEvent(_services));

            return Result.Ok();
        }

        private static string ReadFragmentCodeTemplate()
        {
            const string resourceName = "ADDSMock.Domain.Services.Runtime.FragmentCodeTemplate.template";

            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
