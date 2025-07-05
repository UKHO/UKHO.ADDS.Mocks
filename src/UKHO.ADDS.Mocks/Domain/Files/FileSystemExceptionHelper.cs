namespace UKHO.ADDS.Mocks.Domain.Files
{
    internal static class FileSystemExceptionHelper
    {
        public static FileNotFoundException NewFileNotFoundException(UPath path) => new($"Could not find file `{path}`.");

        public static DirectoryNotFoundException NewDirectoryNotFoundException(UPath path) => new($"Could not find a part of the path `{path}`.");

        public static IOException NewDestinationDirectoryExistException(UPath path) => new($"The destination path `{path}` is an existing directory");

        public static IOException NewDestinationFileExistException(UPath path) => new($"The destination path `{path}` is an existing file");
    }
}
