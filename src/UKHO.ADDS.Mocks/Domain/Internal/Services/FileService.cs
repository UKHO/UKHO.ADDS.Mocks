using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Text;
using UKHO.ADDS.Infrastructure.Results;
using UKHO.ADDS.Mocks.Configuration;
using UKHO.ADDS.Mocks.Domain.Configuration;
using UKHO.ADDS.Mocks.Domain.Internal.Files;
using UKHO.ADDS.Mocks.Files;
using UKHO.ADDS.Mocks.Mime;

namespace UKHO.ADDS.Mocks.Domain.Internal.Services
{
    internal class FileService
    {
        const string TempFilePath = "temp";

        private readonly ObservableCollection<(ServiceDefinition definition, IMockFile file)> _files;
        private readonly IFileSystem _fileSystem;

        public FileService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _files = [];
        }

        public void Initialize()
        {
            if (Directory.Exists(TempFilePath))
            {
                var directoryInfo = new DirectoryInfo(TempFilePath);

                foreach (var file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(TempFilePath);
            }

            foreach (var definition in ServiceRegistry.Definitions)
            {
                foreach (var file in definition.ServiceFiles)
                {
                    _files.Add((definition, file));
                }
            }
        }

        public ObservableCollection<(ServiceDefinition definition, IMockFile file)> Files => _files;

        public IFileSystem FileSystem => _fileSystem;

        public IResult<IMockFile> GetFile(ServiceDefinition definition, string fileName)
        {
            var mockFile = _files.SingleOrDefault(f => f.file.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase) && f.definition.Prefix.Equals(definition.Prefix, StringComparison.InvariantCultureIgnoreCase));

            if (mockFile.file == null)
            {
                return Result.Failure<IMockFile>($"File '{fileName}' not found for mock service '{definition.Name}'.");
            }

            return Result.Success<IMockFile>(mockFile.file);
        }

        public IResult<IMockFile> CreateFile(ServiceDefinition definition, string fileName)
        {
            var path = Path.Combine(TempFilePath, $"{definition.Prefix}-{fileName}");

            if (_fileSystem.File.Exists(path))
            {
                return Result.Failure<IMockFile>($"File '{fileName}' already exists for mock service '{definition.Name}'.");
            }

            var mockFile = new MockFile(fileName, path, MimeTypeMap.GetMimeType(fileName), 0, false, false);
            _files.Add((definition, mockFile));

            return Result.Success<IMockFile>(mockFile);
        }

        public IResult<IMockFile> CreateFile(ServiceDefinition definition, string fileName, Stream content)
        {
            var path = Path.Combine(TempFilePath, $"{definition.Prefix}-{fileName}");

            if (_fileSystem.File.Exists(path))
            {
                return Result.Failure<IMockFile>($"File '{fileName}' already exists for mock service '{definition.Name}'.");
            }

            using var fileStream = _fileSystem.File.Create(path);
            content.CopyTo(fileStream);

            var mockFile = new MockFile(fileName, path, MimeTypeMap.GetMimeType(fileName), fileStream.Length, false, false);
            _files.Add((definition, mockFile));

            return Result.Success<IMockFile>(mockFile);
        }

        public IResult<IMockFile> AppendFile(ServiceDefinition definition, string fileName, byte[] content)
        {
            using var stream = new MemoryStream(content);

            return AppendFile(definition, fileName, stream);
        }

        public IResult<IMockFile> AppendFile(ServiceDefinition definition, string fileName, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            using var stream = new MemoryStream(bytes);

            return AppendFile(definition, fileName, stream);
        }

        public IResult<IMockFile> AppendFile(ServiceDefinition definition, string fileName, Stream content)
        {
            var mockFileResult = GetFile(definition, fileName);

            if (!mockFileResult.IsSuccess(out var mockFile))
            {
                return Result.Failure<IMockFile>($"File '{fileName}' not found for mock service '{definition.Name}'.");
            }

            if (mockFile.IsReadOnly)
            {
                return Result.Failure<IMockFile>($"File '{fileName}' is read-only and cannot be modified.");
            }

            var path = Path.Combine(TempFilePath, $"{definition.Prefix}-{fileName}");

            using var fileStream = _fileSystem.File.OpenWrite(path);
            fileStream.Seek(0, SeekOrigin.End);

            content.CopyTo(fileStream);
            fileStream.Flush();

            ((MockFile)mockFile).Size = fileStream.Length;
            return Result.Success(mockFile);
        }
    }
}
