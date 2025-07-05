﻿// This file is licensed under the BSD-Clause 2 license.


using UKHO.ADDS.Mocks.Domain.Files;
using UKHO.ADDS.Mocks.Domain.Files.FileSystems;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Files.FileSystems
{
    public class TestMemoryFileSystem : TestFileSystemBase
    {
        [Fact]
        public void TestCommonRead()
        {
            var fs = GetCommonMemoryFileSystem();
            AssertCommonRead(fs);
        }

        [Fact]
        public void TestCopyFileSystem()
        {
            var fs = GetCommonMemoryFileSystem();

            var dest = new MemoryFileSystem();
            fs.CopyTo(dest, UPath.Root, true);

            AssertFileSystemEqual(fs, dest);
        }

        [Fact]
        public void TestCopyFileSystemSubFolder()
        {
            var fs = GetCommonMemoryFileSystem();

            var dest = new MemoryFileSystem();
            var subFolder = UPath.Root / "subfolder";
            fs.CopyTo(dest, subFolder, true);

            var destSubFileSystem = dest.GetOrCreateSubFileSystem(subFolder);

            AssertFileSystemEqual(fs, destSubFileSystem);
        }


        [Fact]
        public void TestWatcher()
        {
            var fs = GetCommonMemoryFileSystem();
            AssertFileCreatedEventDispatched(fs, "/a", "/a/watched.txt");
        }

        [Fact]
        public void TestCreatingTopFile()
        {
            var fs = new MemoryFileSystem();
            fs.CreateDirectory("/");
        }

        [Fact]
        public void TestDispose()
        {
            var memfs = new MemoryFileSystem();

            memfs.Dispose();
            Assert.Throws<ObjectDisposedException>(() => memfs.DirectoryExists("/"));
        }

        [Fact]
        public void TestCopyFileCross()
        {
            var fs = new TriggerMemoryFileSystem();
            fs.CreateDirectory("/sub1");
            fs.CreateDirectory("/sub2");
            var sub1 = new SubFileSystem(fs, "/sub1");
            var sub2 = new SubFileSystem(fs, "/sub2");
            sub1.WriteAllText("/file.txt", "test");
            sub1.CopyFileCross("/file.txt", sub2, "/file.txt", false);
            Assert.Equal("test", sub2.ReadAllText("/file.txt"));
            Assert.Equal(TriggerMemoryFileSystem.TriggerType.Copy, fs.Triggered);
        }

        [Fact]
        public void TestMoveFileCross()
        {
            var fs = new TriggerMemoryFileSystem();
            fs.CreateDirectory("/sub1");
            fs.CreateDirectory("/sub2");
            var sub1 = new SubFileSystem(fs, "/sub1");
            var sub2 = new SubFileSystem(fs, "/sub2");
            sub1.WriteAllText("/file.txt", "test");
            sub1.MoveFileCross("/file.txt", sub2, "/file.txt");
            Assert.Equal("test", sub2.ReadAllText("/file.txt"));
            Assert.False(sub1.FileExists("/file.txt"));
            Assert.Equal(TriggerMemoryFileSystem.TriggerType.Move, fs.Triggered);
        }

        [Fact]
        public void TestMoveFileCrossMount()
        {
            var fs = new TriggerMemoryFileSystem();
            fs.CreateDirectory("/sub1");
            fs.CreateDirectory("/sub2");
            var mount = new MountFileSystem();
            var sub1 = new SubFileSystem(fs, "/sub1");
            var sub2 = new SubFileSystem(fs, "/sub2");
            mount.Mount("/sub2-mount", sub2);
            sub1.WriteAllText("/file.txt", "test");
            sub1.MoveFileCross("/file.txt", mount, "/sub2-mount/file.txt");
            Assert.Equal("test", sub2.ReadAllText("/file.txt"));
            Assert.False(sub1.FileExists("/file.txt"));
            Assert.Equal(TriggerMemoryFileSystem.TriggerType.Move, fs.Triggered);
        }

        private sealed class TriggerMemoryFileSystem : MemoryFileSystem
        {
            public enum TriggerType
            {
                None,
                Copy,
                Move
            }

            public TriggerType Triggered { get; private set; } = TriggerType.None;

            protected override void CopyFileImpl(UPath srcPath, UPath destPath, bool overwrite)
            {
                Triggered = TriggerType.Copy;
                base.CopyFileImpl(srcPath, destPath, overwrite);
            }

            protected override void MoveFileImpl(UPath srcPath, UPath destPath)
            {
                Triggered = TriggerType.Move;
                base.MoveFileImpl(srcPath, destPath);
            }
        }
    }
}
