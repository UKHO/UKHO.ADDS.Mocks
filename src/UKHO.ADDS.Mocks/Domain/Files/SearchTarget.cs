namespace UKHO.ADDS.Mocks.Domain.Files
{
    /// <summary>
    ///     Defines the behavior of <see cref="IFileSystem.EnumeratePaths" /> when looking for files and/or folders.
    /// </summary>
    public enum SearchTarget
    {
        /// <summary>
        ///     Search for both files and folders.
        /// </summary>
        Both,

        /// <summary>
        ///     Search for files.
        /// </summary>
        File,

        /// <summary>
        ///     Search for directories.
        /// </summary>
        Directory
    }
}
