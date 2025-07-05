using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using UKHO.ADDS.Mocks.Configuration;
using UKHO.ADDS.Mocks.Domain.Configuration;
using Zio;
using Zio.FileSystems;

namespace UKHO.ADDS.Mocks.Domain.Internal.Services
{
    internal class FileSystemService
    {
        private readonly PhysicalFileSystem _fileSystem;

        private readonly ConcurrentDictionary<ServiceDefinition, IFileSystem> _fileSystems;
        private readonly System.IO.Abstractions.IFileSystem _hostFileSystem;
        private string _rootPath;
        private UPath _rootUPath;

        public FileSystemService(System.IO.Abstractions.IFileSystem hostFileSystem)
        {
            _hostFileSystem = hostFileSystem;
            _fileSystems = new ConcurrentDictionary<ServiceDefinition, IFileSystem>();

            _fileSystem = new PhysicalFileSystem();
        }

        public async Task InitializeAsync(CancellationToken stoppingToken)
        {
            var tempPath = Path.GetTempPath();
            var tempUPath = TranslateToUPath(tempPath);

            _rootUPath = UPath.Combine(tempUPath, "adds-mock-fs");

            // Set up directory structure and copy all static files into position
            _rootPath = _hostFileSystem.Path.Combine(tempPath, "adds-mock-fs");

            if (_hostFileSystem.Directory.Exists(_rootPath))
            {
                ForceDeleteDirectory(_hostFileSystem, _rootPath);
            }

            _hostFileSystem.Directory.CreateDirectory(_rootPath);

            foreach (var definition in ServiceRegistry.Definitions)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                foreach (var file in definition.ServiceFiles)
                {
                    var filePath = _hostFileSystem.Path.Combine(_rootPath, definition.Prefix, file.Name);
                    var directoryPath = _hostFileSystem.Path.GetDirectoryName(filePath);

                    if (!_hostFileSystem.Directory.Exists(directoryPath))
                    {
                        _hostFileSystem.Directory.CreateDirectory(directoryPath);
                    }

                    if (_hostFileSystem.File.Exists(filePath))
                    {
                        File.SetAttributes(filePath, FileAttributes.Normal);
                        _hostFileSystem.File.Delete(filePath);
                    }

                    await using var sourceStream = new FileStream(file.Path, FileMode.Open);
                    await using var destinationStream = _hostFileSystem.File.Create(filePath);

                    var attributes = File.GetAttributes(filePath);
                    attributes |= FileAttributes.ReadOnly;

                    File.SetAttributes(filePath, attributes);

                    await sourceStream.CopyToAsync(destinationStream, stoppingToken);
                }

                GetFileSystem(definition);
            }
        }

        public IReadOnlyDictionary<ServiceDefinition, IFileSystem> FileSystems => _fileSystems;

        /// <summary>
        ///     Gets a <see cref="IFileSystem"></see> for the specified service/>
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public IFileSystem GetFileSystem(ServiceDefinition definition)
        {
            if (_fileSystems.TryGetValue(definition, out var fileSystem))
            {
                return fileSystem;
            }

            var newFileSystem = new SubFileSystem(_fileSystem, UPath.Combine(_rootUPath, definition.Prefix));
            _fileSystems.AddOrUpdate(definition, x => newFileSystem, (x, u) => newFileSystem);

            return newFileSystem;
        }

        /// <summary>
        ///     Translates a Windows-style path into a "/mnt/{drive letter}/..." style path.
        ///     On Linux, returns the input path unchanged.
        /// </summary>
        /// <param name="path">The path to translate.</param>
        /// <returns>The translated path.</returns>
        private static UPath TranslateToUPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Example input: C:\Users\foo\AppData\Local\Temp\
                var root = Path.GetPathRoot(path);
                if (string.IsNullOrWhiteSpace(root) || root.Length < 2 || root[1] != ':')
                {
                    throw new ArgumentException($"Path does not contain a drive letter: {path}");
                }

                var driveLetter = char.ToLowerInvariant(root[0]);

                // Remove the drive root from path
                var relativePath = path.Substring(root.Length);

                // Replace backslashes with forward slashes
                relativePath = relativePath.Replace('\\', '/');

                // Remove any leading slashes
                relativePath = relativePath.TrimStart('/');

                // Build the final mnt path
                var result = $"/mnt/{driveLetter}/{relativePath}";

                // Ensure trailing slash if input had it
                if (path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar))
                {
                    if (!result.EndsWith("/"))
                    {
                        result += "/";
                    }
                }

                return result;
            }

            // On Linux or macOS just return it as-is
            return path;
        }

        private void ForceDeleteDirectory(System.IO.Abstractions.IFileSystem fileSystem, string directoryPath)
        {
            if (!fileSystem.Directory.Exists(directoryPath))
                return;

            foreach (var file in fileSystem.Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
            {
                fileSystem.File.SetAttributes(file, FileAttributes.Normal);
            }

            fileSystem.Directory.Delete(directoryPath, true);
        }
    }
}
