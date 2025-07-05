using UKHO.ADDS.Mocks.Domain.Files.FileSystems;

namespace UKHO.ADDS.Mocks.Tests.Domain.Files.FileSystems
{
    public class TestMountFileSystemCompatSub : TestFileSystemCompactBase
    {
        public TestMountFileSystemCompatSub()
        {
            // Check that MountFileSystem is working with a mount with the compat test
            var mountfs = new MountFileSystem();
            mountfs.Mount("/customMount", new MemoryFileSystem());

            // Use a SubFileSystem to fake the mount to a root folder
            fs = new SubFileSystem(mountfs, "/customMount");
        }
    }
}
