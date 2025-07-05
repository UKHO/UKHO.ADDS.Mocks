using UKHO.ADDS.Mocks.Domain.Files.FileSystems;

namespace UKHO.ADDS.Mocks.Tests.Domain.Files.FileSystems
{
    public class TestMountFileSystemCompat : TestFileSystemCompactBase
    {
        public TestMountFileSystemCompat() =>
            // Check that the MountFileSystem is working with only a plain backup MemoryFileSystem with the compat test
            fs = new MountFileSystem(new MemoryFileSystem());
    }
}
