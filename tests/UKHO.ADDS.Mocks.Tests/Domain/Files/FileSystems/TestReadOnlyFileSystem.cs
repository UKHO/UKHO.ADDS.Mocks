using UKHO.ADDS.Mocks.Domain.Files.FileSystems;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Files.FileSystems
{
    public class TestReadOnlyFileSystem : TestFileSystemBase
    {
        [Fact]
        public void TestCommonReadOnly()
        {
            var rofs = new ReadOnlyFileSystem(GetCommonMemoryFileSystem());
            AssertCommonReadOnly(rofs);
        }
    }
}
