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
        private readonly string _tempFilePath;

        public FileService(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
            Files = [];

            _tempFilePath = Path.Combine(Path.GetTempPath(), "adds-mock");
        }

        public ObservableCollection<(ServiceDefinition definition, IMockFile file)> Files { get; }

        public IFileSystem FileSystem { get; }

        public void Initialize()
        {
            if (Directory.Exists(_tempFilePath))
            {
                var directoryInfo = new DirectoryInfo(_tempFilePath);

                foreach (var file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(_tempFilePath);
            }

            foreach (var definition in ServiceRegistry.Definitions)
            {
                foreach (var file in definition.ServiceFiles)
                {
                    Files.Add((definition, file));
                }
            }
        }

        public IResult<IMockFile> GetFile(ServiceDefinition definition, string fileName)
        {
            var mockFile = Files.SingleOrDefault(f => f.file.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase) && f.definition.Prefix.Equals(definition.Prefix, StringComparison.InvariantCultureIgnoreCase));

            if (mockFile.file == null)
            {
                return Result.Failure<IMockFile>($"File '{fileName}' not found for mock service '{definition.Name}'.");
            }

            return Result.Success(mockFile.file);
        }

        public IResult<IMockFile> CreateFile(ServiceDefinition definition, string fileName)
        {
            var path = Path.Combine(_tempFilePath, $"{definition.Prefix}-{fileName}");

            if (FileSystem.File.Exists(path))
            {
                return Result.Failure<IMockFile>($"File '{fileName}' already exists for mock service '{definition.Name}'.");
            }

            var mockFile = new MockFile(fileName, path, MimeTypeMap.GetMimeType(fileName), 0, false, false);
            Files.Add((definition, mockFile));

            return Result.Success<IMockFile>(mockFile);
        }

        public IResult<IMockFile> CreateFile(ServiceDefinition definition, string fileName, Stream content)
        {
            var path = Path.Combine(_tempFilePath, $"{definition.Prefix}-{fileName}");

            if (FileSystem.File.Exists(path))
            {
                return Result.Failure<IMockFile>($"File '{fileName}' already exists for mock service '{definition.Name}'.");
            }

            using var fileStream = FileSystem.File.Create(path);
            content.CopyTo(fileStream);

            var mockFile = new MockFile(fileName, path, MimeTypeMap.GetMimeType(fileName), fileStream.Length, false, false);
            Files.Add((definition, mockFile));

            return Result.Success<IMockFile>(mockFile);
        }

        public IResult<IMockFile> AppendFile(ServiceDefinition definition, string fileName, byte[] content, bool createIfNotExists)
        {
            using var stream = new MemoryStream(content);

            return AppendFile(definition, fileName, stream, createIfNotExists);
        }

        public IResult<IMockFile> AppendFile(ServiceDefinition definition, string fileName, string content, bool createIfNotExists)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            using var stream = new MemoryStream(bytes);

            return AppendFile(definition, fileName, stream, createIfNotExists);
        }

        public IResult<IMockFile> AppendFile(ServiceDefinition definition, string fileName, Stream content, bool createIfNotExists)
        {
            var mockFileResult = GetFile(definition, fileName);

            if (!mockFileResult.IsSuccess(out var mockFile))
            {
                if (createIfNotExists)
                {
                    return CreateFile(definition, fileName, content);
                }

                return Result.Failure<IMockFile>($"File '{fileName}' not found for mock service '{definition.Name}'.");
            }

            if (mockFile.IsReadOnly)
            {
                return Result.Failure<IMockFile>($"File '{fileName}' is read-only and cannot be modified.");
            }

            var path = Path.Combine(_tempFilePath, $"{definition.Prefix}-{fileName}");

            using var fileStream = FileSystem.File.OpenWrite(path);
            fileStream.Seek(0, SeekOrigin.End);

            content.CopyTo(fileStream);
            fileStream.Flush();

            ((MockFile)mockFile).Size = fileStream.Length;
            return Result.Success(mockFile);
        }
    }
}
