using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Files.FileSystems
{
    public class TestPhysicalFileSystemCompat : TestFileSystemCompactBase
    {
        private readonly PhysicalDirectoryHelper _fsHelper;

        public TestPhysicalFileSystemCompat()
        {
            _fsHelper = new PhysicalDirectoryHelper(SystemPath);
            fs = _fsHelper.PhysicalFileSystem;
        }

        public override void TestDirectoryDeleteAndOpenFileOnWindows()
        {
            Skip.IfNot(IsWindows, "Linux allows files to be deleted when they are open");

            base.TestDirectoryDeleteAndOpenFileOnWindows();
        }

        public override void Dispose()
        {
            _fsHelper.Dispose();
            base.Dispose();
        }
    }
}
