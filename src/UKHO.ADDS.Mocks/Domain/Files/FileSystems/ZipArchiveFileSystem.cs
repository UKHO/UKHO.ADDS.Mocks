﻿using System.Diagnostics;
using System.IO.Compression;

namespace UKHO.ADDS.Mocks.Domain.Files.FileSystems
{
    /// <summary>
    ///     Provides a <see cref="IFileSystem" /> for the ZipArchive filesystem.
    /// </summary>
    public class ZipArchiveFileSystem : FileSystem
    {
        private const char DirectorySeparator = '/';

        private static readonly char[] s_slashChars = { '/', '\\' };

        private readonly CompressionLevel _compressionLevel;

        private readonly DateTime _creationTime;
        private readonly object _dispatcherLock = new();
        private readonly bool _disposeStream;

        private readonly ReaderWriterLockSlim _entriesLock = new();
        private readonly bool _isCaseSensitive;

        private readonly Dictionary<ZipArchiveEntry, EntryState> _openStreams;
        private readonly object _openStreamsLock = new();

        private readonly string? _path;
        private readonly Stream? _stream;

        private ZipArchive _archive;

        private FileSystemEventDispatcher<FileSystemWatcher>? _dispatcher;
        private Dictionary<UPath, InternalZipEntry> _entries;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZipArchiveFileSystem" /> class.
        /// </summary>
        /// <param name="archive">An instance of <see cref="ZipArchive" /></param>
        /// <param name="isCaseSensitive">Specifies if entry names should be case sensitive</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ZipArchiveFileSystem(ZipArchive archive, bool isCaseSensitive = false, CompressionLevel compressionLevel = CompressionLevel.NoCompression)
        {
            _archive = archive;
            _isCaseSensitive = isCaseSensitive;
            _creationTime = DateTime.Now;
            _compressionLevel = compressionLevel;
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            _openStreams = new Dictionary<ZipArchiveEntry, EntryState>();
            _entries = null!; // Loaded below
            LoadEntries();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZipArchiveFileSystem" /> class.
        /// </summary>
        /// <param name="stream">Instance of stream to create <see cref="ZipArchive" /> from</param>
        /// <param name="mode">Mode of <see cref="ZipArchive" /></param>
        /// <param name="leaveOpen">True to leave the stream open when <see cref="ZipArchive" /> is disposed</param>
        /// <param name="isCaseSensitive"></param>
        public ZipArchiveFileSystem(Stream stream, ZipArchiveMode mode = ZipArchiveMode.Update, bool leaveOpen = false, bool isCaseSensitive = false, CompressionLevel compressionLevel = CompressionLevel.NoCompression)
            : this(new ZipArchive(stream, mode, true), isCaseSensitive, compressionLevel)
        {
            _disposeStream = !leaveOpen;
            _stream = stream;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZipArchiveFileSystem" /> class from file.
        /// </summary>
        /// <param name="path">Path to zip file</param>
        /// <param name="mode">Mode of <see cref="ZipArchive" /></param>
        /// <param name="leaveOpen">True to leave the stream open when <see cref="ZipArchive" /> is disposed</param>
        /// <param name="isCaseSensitive">Specifies if entry names should be case sensitive</param>
        public ZipArchiveFileSystem(string path, ZipArchiveMode mode = ZipArchiveMode.Update, bool leaveOpen = false, bool isCaseSensitive = false, CompressionLevel compressionLevel = CompressionLevel.NoCompression)
            : this(new ZipArchive(File.Open(path, FileMode.OpenOrCreate), mode, leaveOpen), isCaseSensitive, compressionLevel) =>
            _path = path;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZipArchiveFileSystem" /> class with a <see cref="MemoryStream" />
        /// </summary>
        /// <param name="mode">Mode of <see cref="ZipArchive" /></param>
        /// <param name="leaveOpen">True to leave the stream open when <see cref="ZipArchive" /> is disposed</param>
        /// <param name="isCaseSensitive">Specifies if entry names should be case sensitive</param>
        public ZipArchiveFileSystem(ZipArchiveMode mode = ZipArchiveMode.Update, bool leaveOpen = false, bool isCaseSensitive = false, CompressionLevel compressionLevel = CompressionLevel.NoCompression)
            : this(new MemoryStream(), mode, leaveOpen, isCaseSensitive, compressionLevel)
        {
        }

        /// <summary>
        ///     Saves the archive to the original path or stream.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot save archive without a path or stream</exception>
        public void Save()
        {
            var mode = _archive.Mode;

            if (_path != null)
            {
                _archive.Dispose();
                _archive = new ZipArchive(File.Open(_path, FileMode.OpenOrCreate), mode);
            }
            else if (_stream != null)
            {
                if (!_stream.CanSeek)
                {
                    throw new InvalidOperationException("Cannot save archive to a stream that doesn't support seeking");
                }

                _archive.Dispose();
                _stream.Seek(0, SeekOrigin.Begin);
                _archive = new ZipArchive(_stream, mode, true);
            }
            else
            {
                throw new InvalidOperationException("Cannot save archive without a path or stream");
            }

            LoadEntries();
        }

        private void LoadEntries()
        {
            var comparer = _isCaseSensitive ? UPathComparer.Ordinal : UPathComparer.OrdinalIgnoreCase;

            _entries = _archive.Entries.ToDictionary(
                e => new UPath(e.FullName).ToAbsolute(),
                static e =>
                {
                    var lastChar = e.FullName[e.FullName.Length - 1];
                    return new InternalZipEntry(e, lastChar is '/' or '\\');
                },
                comparer);
        }

        private ZipArchiveEntry? GetEntry(UPath path, out bool isDirectory)
        {
            _entriesLock.EnterReadLock();
            try
            {
                if (_entries.TryGetValue(path, out var foundEntry))
                {
                    isDirectory = foundEntry.IsDirectory;
                    return foundEntry.Entry;
                }
            }
            finally
            {
                _entriesLock.ExitReadLock();
            }

            isDirectory = false;
            return null;
        }

        private ZipArchiveEntry? GetEntry(UPath path) => GetEntry(path, out _);

        /// <inheritdoc />
        protected override UPath ConvertPathFromInternalImpl(string innerPath) => new(innerPath);

        /// <inheritdoc />
        protected override string ConvertPathToInternalImpl(UPath path) => path.FullName;

        /// <inheritdoc />
        protected override void CopyFileImpl(UPath srcPath, UPath destPath, bool overwrite)
        {
            if (srcPath == destPath)
            {
                throw new IOException("Source and destination path must be different.");
            }

            var srcEntry = GetEntry(srcPath, out var isDirectory);

            if (isDirectory)
            {
                throw new UnauthorizedAccessException(nameof(srcPath) + " is a directory.");
            }

            if (srcEntry == null)
            {
                if (!DirectoryExistsImpl(srcPath.GetDirectoryAsSpan()))
                {
                    throw new DirectoryNotFoundException(srcPath.GetDirectory().FullName);
                }

                throw FileSystemExceptionHelper.NewFileNotFoundException(srcPath);
            }

            var parentDirectory = destPath.GetDirectoryAsSpan();
            if (!DirectoryExistsImpl(parentDirectory))
            {
                throw FileSystemExceptionHelper.NewDirectoryNotFoundException(parentDirectory.ToString());
            }

            if (DirectoryExistsImpl(destPath))
            {
                if (!FileExistsImpl(destPath))
                {
                    throw new IOException("Destination path is a directory");
                }
            }

            var destEntry = GetEntry(destPath);
            if (destEntry != null)
            {
                if ((destEntry.ExternalAttributes & (int)FileAttributes.ReadOnly) == (int)FileAttributes.ReadOnly)
                {
                    throw new UnauthorizedAccessException("Destination file is read only");
                }

                if (!overwrite)
                {
                    throw FileSystemExceptionHelper.NewDestinationFileExistException(srcPath);
                }

                RemoveEntry(destEntry);
                TryGetDispatcher()?.RaiseDeleted(destPath);
            }

            destEntry = CreateEntry(destPath.FullName);
            using (var destStream = destEntry.Open())
            {
                using var srcStream = srcEntry.Open();
                srcStream.CopyTo(destStream);
            }

            destEntry.ExternalAttributes = srcEntry.ExternalAttributes | (int)FileAttributes.Archive;

            TryGetDispatcher()?.RaiseCreated(destPath);
        }

        /// <inheritdoc />
        protected override void CreateDirectoryImpl(UPath path)
        {
            if (FileExistsImpl(path))
            {
                throw FileSystemExceptionHelper.NewDestinationFileExistException(path);
            }

            if (DirectoryExistsImpl(path))
            {
                throw FileSystemExceptionHelper.NewDestinationDirectoryExistException(path);
            }

            var parentPath = GetParent(path.AsSpan());
            if (!parentPath.IsEmpty)
            {
                if (!DirectoryExistsImpl(parentPath))
                {
                    CreateDirectoryImpl(parentPath.ToString());
                }
            }

            CreateEntry(path, true);
            TryGetDispatcher()?.RaiseCreated(path);
        }

        /// <inheritdoc />
        protected override void DeleteDirectoryImpl(UPath path, bool isRecursive)
        {
            if (FileExistsImpl(path))
            {
                throw new IOException(nameof(path) + " is a file.");
            }

            var entries = new List<ZipArchiveEntry>();
            if (!isRecursive)
            {
                // folder name ends with slash so StartWith check is enough
                _entriesLock.EnterReadLock();
                try
                {
                    entries = _entries
                        .Where(x => x.Key.FullName.StartsWith(path.FullName, _isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                        .Take(2)
                        .Select(x => x.Value.Entry)
                        .ToList();
                }
                finally
                {
                    _entriesLock.ExitReadLock();
                }

                if (entries.Count == 0)
                {
                    throw FileSystemExceptionHelper.NewDirectoryNotFoundException(path);
                }

                if (entries.Count == 1)
                {
                    RemoveEntry(entries[0]);
                }

                if (entries.Count == 2)
                {
                    throw new IOException("Directory is not empty");
                }

                TryGetDispatcher()?.RaiseDeleted(path);
                return;
            }

            _entriesLock.EnterReadLock();
            try
            {
                entries = _entries
                    .Where(x => x.Key.FullName.StartsWith(path.FullName, _isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Value.Entry)
                    .ToList();

                if (entries.Count == 0)
                {
                    throw FileSystemExceptionHelper.NewDirectoryNotFoundException(path);
                }

                // check if there are no open file in directory
                foreach (var entry in entries)
                {
                    lock (_openStreamsLock)
                    {
                        if (_openStreams.ContainsKey(entry))
                        {
                            throw new IOException($"There is an open file {entry.FullName} in directory");
                        }
                    }
                }

                // check if there are none readonly entries
                foreach (var entry in entries)
                {
                    if ((entry.ExternalAttributes & (int)FileAttributes.ReadOnly) == (int)FileAttributes.ReadOnly)
                    {
                        throw entry.FullName.Length == path.FullName.Length + 1
                            ? new IOException("Directory is read only")
                            : new UnauthorizedAccessException($"Cannot delete directory that contains readonly entry {entry.FullName}");
                    }
                }
            }
            finally
            {
                _entriesLock.ExitReadLock();
            }

            _entriesLock.EnterWriteLock();
            try
            {
                foreach (var entry in entries)
                {
                    _entries.Remove(new UPath(entry.FullName).ToAbsolute());
                    entry.Delete();
                }
            }
            finally
            {
                _entriesLock.ExitWriteLock();
            }

            TryGetDispatcher()?.RaiseDeleted(path);
        }

        /// <inheritdoc />
        protected override void DeleteFileImpl(UPath path)
        {
            if (DirectoryExistsImpl(path))
            {
                throw new IOException("Cannot delete a directory");
            }

            var entry = GetEntry(path);
            if (entry == null)
            {
                return;
            }

            if ((entry.ExternalAttributes & (int)FileAttributes.ReadOnly) == (int)FileAttributes.ReadOnly)
            {
                throw new UnauthorizedAccessException("Cannot delete file with readonly attribute");
            }

            TryGetDispatcher()?.RaiseDeleted(path);
            RemoveEntry(entry);
        }

        /// <inheritdoc />
        protected override bool DirectoryExistsImpl(UPath path) => DirectoryExistsImpl(path.FullName.AsSpan());

        private bool DirectoryExistsImpl(ReadOnlySpan<char> path)
        {
            if (path is "/" or "\\" or "")
            {
                return true;
            }

            _entriesLock.EnterReadLock();

            try
            {
                return _entries.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(path, out var entry) && entry.IsDirectory;
            }
            finally
            {
                _entriesLock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            _archive.Dispose();

            if (_stream != null && _disposeStream)
            {
                _stream.Dispose();
            }

            if (disposing)
            {
                TryGetDispatcher()?.Dispose();
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<FileSystemItem> EnumerateItemsImpl(UPath path, SearchOption searchOption, SearchPredicate? searchPredicate) => EnumeratePathsStr(path, "*", searchOption, SearchTarget.Both).Select(p => new FileSystemItem(this, p, p[p.Length - 1] == DirectorySeparator));

        /// <inheritdoc />
        protected override IEnumerable<UPath> EnumeratePathsImpl(UPath path, string searchPattern, SearchOption searchOption, SearchTarget searchTarget) => EnumeratePathsStr(path, searchPattern, searchOption, searchTarget).Select(x => new UPath(x));

        private IEnumerable<string> EnumeratePathsStr(UPath path, string searchPattern, SearchOption searchOption, SearchTarget searchTarget)
        {
            var search = SearchPattern.Parse(ref path, ref searchPattern);

            _entriesLock.EnterReadLock();
            var entriesList = new List<ZipArchiveEntry>();
            try
            {
                var internEntries = path == UPath.Root
                    ? _entries
                    : _entries.Where(kv => kv.Key.FullName.StartsWith(path.FullName, _isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) && kv.Key.FullName.Length > path.FullName.Length);

                if (searchOption == SearchOption.TopDirectoryOnly)
                {
                    internEntries = internEntries.Where(kv => kv.Key.IsInDirectory(path, false));
                }

                entriesList = internEntries.Select(kv => kv.Value.Entry).ToList();
            }
            finally
            {
                _entriesLock.ExitReadLock();
            }

            if (entriesList.Count == 0)
            {
                return Enumerable.Empty<string>();
            }

            var entries = (IEnumerable<ZipArchiveEntry>)entriesList;

            if (searchTarget == SearchTarget.File)
            {
                entries = entries.Where(e => e.FullName[e.FullName.Length - 1] != DirectorySeparator);
            }
            else if (searchTarget == SearchTarget.Directory)
            {
                entries = entries.Where(e => e.FullName[e.FullName.Length - 1] == DirectorySeparator);
            }

            if (!string.IsNullOrEmpty(searchPattern))
            {
                entries = entries.Where(e => search.Match(GetName(e)));
            }

            return entries.Select(e => '/' + e.FullName);
        }

        /// <inheritdoc />
        protected override bool FileExistsImpl(UPath path)
        {
            _entriesLock.EnterReadLock();

            try
            {
                return _entries.TryGetValue(path, out var entry) && !entry.IsDirectory;
            }
            finally
            {
                _entriesLock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        protected override FileAttributes GetAttributesImpl(UPath path)
        {
            var entry = GetEntry(path);
            if (entry is null)
            {
                throw FileSystemExceptionHelper.NewFileNotFoundException(path);
            }

            var attributes = entry.FullName[entry.FullName.Length - 1] == DirectorySeparator ? FileAttributes.Directory : 0;

            const FileAttributes validValues = (FileAttributes)0x7FFF /* Up to FileAttributes.Encrypted */ | FileAttributes.IntegrityStream | FileAttributes.NoScrubData;
            var externalAttributes = (FileAttributes)entry.ExternalAttributes & validValues;

            if (externalAttributes == 0 && attributes == 0)
            {
                attributes |= FileAttributes.Normal;
            }

            return externalAttributes | attributes;
        }

        /// <inheritdoc />
        protected override long GetFileLengthImpl(UPath path)
        {
            var entry = GetEntry(path, out var isDirectory);

            if (entry == null || isDirectory)
            {
                throw FileSystemExceptionHelper.NewFileNotFoundException(path);
            }

            try
            {
                return entry.Length;
            }
            catch (Exception ex) // for some reason entry.Length doesn't work with MemoryStream used in tests
            {
                Debug.WriteLine(ex.Message);
                using var stream = OpenFileImpl(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return stream.Length;
            }
        }

        /// <summary>
        ///     Not supported by zip format. Return last write time.
        /// </summary>
        protected override DateTime GetCreationTimeImpl(UPath path) => GetLastWriteTimeImpl(path);

        /// <summary>
        ///     Not supported by zip format. Return last write time
        /// </summary>
        protected override DateTime GetLastAccessTimeImpl(UPath path) => GetLastWriteTimeImpl(path);

        /// <inheritdoc />
        protected override DateTime GetLastWriteTimeImpl(UPath path)
        {
            var entry = GetEntry(path);
            if (entry == null)
            {
                return DefaultFileTime;
            }

            return entry.LastWriteTime.DateTime;
        }

        /// <inheritdoc />
        protected override void MoveDirectoryImpl(UPath srcPath, UPath destPath)
        {
            if (destPath.IsInDirectory(srcPath, true))
            {
                throw new IOException("Cannot move directory to itself or a subdirectory.");
            }

            if (FileExistsImpl(srcPath))
            {
                throw new IOException(nameof(srcPath) + " is a file.");
            }

            var srcDir = srcPath.FullName;

            _entriesLock.EnterReadLock();
            var entries = Array.Empty<ZipArchiveEntry>();
            try
            {
                entries = _archive.Entries.Where(e => e.FullName.StartsWith(srcDir, _isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            finally
            {
                _entriesLock.ExitReadLock();
            }

            if (entries.Length == 0)
            {
                throw FileSystemExceptionHelper.NewDirectoryNotFoundException(srcPath);
            }

            CreateDirectoryImpl(destPath);
            foreach (var entry in entries)
            {
                if (entry.FullName.Length == srcDir.Length)
                {
                    RemoveEntry(entry);
                    continue;
                }

                using (var entryStream = entry.Open())
                {
                    var entryName = entry.FullName.Substring(srcDir.Length);
                    var destEntry = CreateEntry(destPath + entryName, true);
                    using (var destEntryStream = destEntry.Open())
                    {
                        entryStream.CopyTo(destEntryStream);
                    }
                }

                TryGetDispatcher()?.RaiseCreated(destPath);
                RemoveEntry(entry);
                TryGetDispatcher()?.RaiseDeleted(srcPath);
            }
        }

        /// <inheritdoc />
        protected override void MoveFileImpl(UPath srcPath, UPath destPath)
        {
            var srcEntry = GetEntry(srcPath) ?? throw FileSystemExceptionHelper.NewFileNotFoundException(srcPath);

            if (!DirectoryExistsImpl(destPath.GetDirectoryAsSpan()))
            {
                throw FileSystemExceptionHelper.NewDirectoryNotFoundException(destPath.GetDirectory());
            }

            var destEntry = GetEntry(destPath);
            if (destEntry != null)
            {
                throw new IOException("Cannot overwrite existing file.");
            }

            destEntry = CreateEntry(destPath.FullName);
            TryGetDispatcher()?.RaiseCreated(destPath);
            using (var destStream = destEntry.Open())
            {
                using var srcStream = srcEntry.Open();
                srcStream.CopyTo(destStream);
            }

            RemoveEntry(srcEntry);
            TryGetDispatcher()?.RaiseDeleted(srcPath);
        }

        /// <inheritdoc />
        protected override Stream OpenFileImpl(UPath path, FileMode mode, FileAccess access, FileShare share)
        {
            if (_archive.Mode == ZipArchiveMode.Read && access == FileAccess.Write)
            {
                throw new UnauthorizedAccessException("Cannot open a file for writing in a read-only archive.");
            }

            if (access == FileAccess.Read && (mode == FileMode.CreateNew || mode == FileMode.Create || mode == FileMode.Truncate || mode == FileMode.Append))
            {
                throw new ArgumentException("Cannot write in a read-only access.");
            }

            var entry = GetEntry(path, out var isDirectory);

            if (isDirectory)
            {
                throw new UnauthorizedAccessException(nameof(path) + " is a directory.");
            }

            if (entry == null)
            {
                if (mode is FileMode.Create or FileMode.CreateNew or FileMode.OpenOrCreate or FileMode.Append)
                {
                    entry = CreateEntry(path.FullName);

                    TryGetDispatcher()?.RaiseCreated(path);
                }
                else
                {
                    if (!DirectoryExistsImpl(path.GetDirectoryAsSpan()))
                    {
                        throw FileSystemExceptionHelper.NewDirectoryNotFoundException(path.GetDirectory());
                    }

                    throw FileSystemExceptionHelper.NewFileNotFoundException(path);
                }
            }
            else if (mode == FileMode.CreateNew)
            {
                throw new IOException("Cannot create a file in CreateNew mode if it already exists.");
            }
            else if (mode == FileMode.Create)
            {
                RemoveEntry(entry);
                entry = CreateEntry(path.FullName);
            }

            if ((access == FileAccess.Write || access == FileAccess.ReadWrite) && (entry.ExternalAttributes & (int)FileAttributes.ReadOnly) == (int)FileAttributes.ReadOnly)
            {
                throw new UnauthorizedAccessException("Cannot open a file for writing in a file with readonly attribute.");
            }

            var stream = new ZipEntryStream(share, this, entry);

            if (access is FileAccess.Write or FileAccess.ReadWrite)
            {
                entry.ExternalAttributes |= (int)FileAttributes.Archive;
            }

            if (mode == FileMode.Append)
            {
                stream.Seek(0, SeekOrigin.End);
            }
            else if (mode == FileMode.Truncate)
            {
                stream.SetLength(0);
            }

            return stream;
        }

        /// <inheritdoc />
        protected override void ReplaceFileImpl(UPath srcPath, UPath destPath, UPath destBackupPath, bool ignoreMetadataErrors)
        {
            var sourceEntry = GetEntry(srcPath);
            if (sourceEntry is null)
            {
                throw FileSystemExceptionHelper.NewFileNotFoundException(srcPath);
            }

            var destEntry = GetEntry(destPath);
            if (destEntry == sourceEntry)
            {
                throw new IOException("Cannot replace the file with itself.");
            }

            if (destEntry != null)
            {
                // create a backup at destBackupPath if its not null
                if (!destBackupPath.IsEmpty)
                {
                    var destBackupEntry = CreateEntry(destBackupPath.FullName);
                    using var destBackupStream = destBackupEntry.Open();
                    using var destStream = destEntry.Open();
                    destStream.CopyTo(destBackupStream);
                }

                RemoveEntry(destEntry);
            }

            var newEntry = CreateEntry(destPath.FullName);
            using (var newStream = newEntry.Open())
            {
                using (var sourceStream = sourceEntry.Open())
                {
                    sourceStream.CopyTo(newStream);
                }
            }

            RemoveEntry(sourceEntry);
            TryGetDispatcher()?.RaiseDeleted(srcPath);
            TryGetDispatcher()?.RaiseCreated(destPath);
        }

        /// <summary>
        ///     Implementation for <see cref="SetAttributes" />, <paramref name="path" /> is guaranteed to be absolute and
        ///     validated through <see cref="ValidatePath" />. Works only in Net Standard 2.1
        ///     Sets the specified <see cref="FileAttributes" /> of the file or directory on the specified path.
        /// </summary>
        /// <param name="path">The path to the file or directory.</param>
        /// <param name="attributes">A bitwise combination of the enumeration values.</param>
        protected override void SetAttributesImpl(UPath path, FileAttributes attributes)
        {
            var entry = GetEntry(path);
            if (entry == null)
            {
                throw FileSystemExceptionHelper.NewFileNotFoundException(path);
            }

            entry.ExternalAttributes = (int)attributes;
            TryGetDispatcher()?.RaiseChange(path);
        }

        /// <summary>
        ///     Not supported by zip format. Does nothing.
        /// </summary>
        protected override void SetCreationTimeImpl(UPath path, DateTime time)
        {
        }

        /// <summary>
        ///     Not supported by zip format. Does nothing.
        /// </summary>
        protected override void SetLastAccessTimeImpl(UPath path, DateTime time)
        {
        }

        /// <inheritdoc />
        protected override void SetLastWriteTimeImpl(UPath path, DateTime time)
        {
            var entry = GetEntry(path);
            if (entry is null)
            {
                throw FileSystemExceptionHelper.NewFileNotFoundException(path);
            }

            TryGetDispatcher()?.RaiseChange(path);
            entry.LastWriteTime = time;
        }

        /// <inheritdoc />
        protected override void CreateSymbolicLinkImpl(UPath path, UPath pathToTarget) => throw new NotSupportedException("Symbolic links are not supported by ZipArchiveFileSystem");

        /// <inheritdoc />
        protected override bool TryResolveLinkTargetImpl(UPath linkPath, out UPath resolvedPath)
        {
            resolvedPath = UPath.Empty;
            return false;
        }

        /// <inheritdoc />
        protected override IFileSystemWatcher WatchImpl(UPath path)
        {
            var watcher = new FileSystemWatcher(this, path);
            lock (_dispatcherLock)
            {
                _dispatcher ??= new FileSystemEventDispatcher<FileSystemWatcher>(this);
                _dispatcher.Add(watcher);
            }

            return watcher;
        }

        private void RemoveEntry(ZipArchiveEntry entry)
        {
            _entriesLock.EnterWriteLock();
            try
            {
                entry.Delete();
                _entries.Remove(new UPath(entry.FullName).ToAbsolute());
            }
            finally
            {
                _entriesLock.ExitWriteLock();
            }
        }

        private ZipArchiveEntry CreateEntry(UPath path, bool isDirectory = false)
        {
            _entriesLock.EnterWriteLock();
            try
            {
                var internalPath = path.FullName;

                if (isDirectory)
                {
                    internalPath += DirectorySeparator;
                }

                var entry = _archive.CreateEntry(internalPath, _compressionLevel);
                _entries[path] = new InternalZipEntry(entry, isDirectory);
                return entry;
            }
            finally
            {
                _entriesLock.ExitWriteLock();
            }
        }

        private static ReadOnlySpan<char> GetName(ZipArchiveEntry entry)
        {
            var name = entry.FullName.TrimEnd(s_slashChars);
            var index = name.LastIndexOfAny(s_slashChars);
            return index == -1 ? name.AsSpan() : name.AsSpan(index + 1);
        }

        private static ReadOnlySpan<char> GetParent(ReadOnlySpan<char> path)
        {
            path = path.TrimEnd(s_slashChars);
            var lastIndex = path.LastIndexOfAny(s_slashChars);
            return lastIndex == -1 ? ReadOnlySpan<char>.Empty : path.Slice(0, lastIndex);
        }

        private FileSystemEventDispatcher<FileSystemWatcher>? TryGetDispatcher()
        {
            lock (_dispatcherLock)
            {
                return _dispatcher;
            }
        }

        private sealed class ZipEntryStream : Stream
        {
            private readonly ZipArchiveEntry _entry;
            private readonly ZipArchiveFileSystem _fileSystem;
            private readonly Stream _streamImplementation;
            private bool _isDisposed;

            public ZipEntryStream(FileShare share, ZipArchiveFileSystem system, ZipArchiveEntry entry)
            {
                _entry = entry;
                _fileSystem = system;

                lock (_fileSystem._openStreamsLock)
                {
                    var fileShare = _fileSystem._openStreams.TryGetValue(entry, out var fileData) ? fileData.Share : FileShare.ReadWrite;
                    if (fileData != null)
                    {
                        // we only check for read share, because ZipArchive doesn't support write share
                        if (share is not FileShare.Read and not FileShare.ReadWrite)
                        {
                            throw new IOException("File is already opened for reading");
                        }

                        if (fileShare is not FileShare.Read and not FileShare.ReadWrite)
                        {
                            throw new IOException("File is already opened for reading by another stream with non compatible share");
                        }

                        fileData.Count++;
                    }
                    else
                    {
                        _fileSystem._openStreams.Add(_entry, new EntryState(share));
                    }

                    _streamImplementation = entry.Open();
                }

                Share = share;
            }

            private FileShare Share { get; }

            public override bool CanRead => _streamImplementation.CanRead;

            public override bool CanSeek => _streamImplementation.CanSeek;

            public override bool CanWrite => _streamImplementation.CanWrite;

            public override long Length => _streamImplementation.Length;

            public override long Position
            {
                get => _streamImplementation.Position;
                set => _streamImplementation.Position = value;
            }

            public override void Flush() => _streamImplementation.Flush();

            public override int Read(byte[] buffer, int offset, int count) => _streamImplementation.Read(buffer, offset, count);

            public override long Seek(long offset, SeekOrigin origin) => _streamImplementation.Seek(offset, origin);

            public override void SetLength(long value) => _streamImplementation.SetLength(value);

            public override void Write(byte[] buffer, int offset, int count) => _streamImplementation.Write(buffer, offset, count);

            public override void Close()
            {
                if (_isDisposed)
                {
                    return;
                }

                _streamImplementation.Close();
                _isDisposed = true;
                lock (_fileSystem._openStreamsLock)
                {
                    if (!_fileSystem._openStreams.TryGetValue(_entry, out var fileData))
                    {
                        return;
                    }

                    fileData.Count--;
                    if (fileData.Count == 0)
                    {
                        _fileSystem._openStreams.Remove(_entry);
                    }
                }
            }
        }

        private sealed class EntryState
        {
            public int Count;

            public EntryState(FileShare share)
            {
                Share = share;
                Count = 1;
            }

            public FileShare Share { get; }
        }

        private readonly struct InternalZipEntry
        {
            public InternalZipEntry(ZipArchiveEntry entry, bool isDirectory)
            {
                Entry = entry;
                IsDirectory = isDirectory;
            }

            public readonly ZipArchiveEntry Entry;
            public readonly bool IsDirectory;
        }
    }
}
