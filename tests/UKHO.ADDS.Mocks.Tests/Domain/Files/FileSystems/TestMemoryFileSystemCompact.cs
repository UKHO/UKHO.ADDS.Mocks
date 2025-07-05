using UKHO.ADDS.Mocks.Domain.Files.FileSystems;

namespace UKHO.ADDS.Mocks.Tests.Domain.Files.FileSystems
{
    public class TestMemoryFileSystemCompact : TestFileSystemCompactBase
    {
        public TestMemoryFileSystemCompact() => fs = new MemoryFileSystem();
    }
}
