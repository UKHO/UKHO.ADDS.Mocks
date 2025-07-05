namespace UKHO.ADDS.Mocks.Domain.Files
{
    /// <summary>
    ///     Represents a file or directory rename event.
    /// </summary>
    /// <inheritdoc />
    public class FileRenamedEventArgs : FileChangedEventArgs
    {
        public FileRenamedEventArgs(IFileSystem fileSystem, WatcherChangeTypes changeType, UPath fullPath, UPath oldFullPath)
            : base(fileSystem, changeType, fullPath)
        {
            fullPath.AssertNotNull(nameof(oldFullPath));
            fullPath.AssertAbsolute(nameof(oldFullPath));

            OldFullPath = oldFullPath;
            OldName = oldFullPath.GetName();
        }

        /// <summary>
        ///     Absolute path to the old location of the file or directory.
        /// </summary>
        /// v
        public UPath OldFullPath { get; }

        /// <summary>
        ///     Old name of the file or directory.
        /// </summary>
        public string OldName { get; }
    }
}
