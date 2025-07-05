namespace UKHO.ADDS.Mocks.Domain.Files
{
    /// <summary>
    ///     Contains information about a filesystem error event.
    /// </summary>
    /// <inheritdoc />
    public class FileSystemErrorEventArgs : EventArgs
    {
        public FileSystemErrorEventArgs(Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Exception = exception;
        }

        /// <summary>
        ///     Exception that was thrown in the filesystem.
        /// </summary>
        public Exception Exception { get; }
    }
}
