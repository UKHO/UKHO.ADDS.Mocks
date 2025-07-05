using UKHO.ADDS.Mocks.Domain.Files;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Files
{
    public class TestUPathExtension
    {
        [Theory]
        [InlineData("/a/b", "a", "b")]
        [InlineData("/a/b/c", "a", "b/c")]
        [InlineData("a/b", "a", "b")]
        [InlineData("a/b/c", "a", "b/c")]
        [InlineData("", "", "")]
        [InlineData("/z", "z", "")]
        public void TestGetFirstDirectory(string path, string expectedFirstDir, string expectedRest)
        {
            var pathInfo = new UPath(path);
            var firstDir = pathInfo.GetFirstDirectory(out var rest);
            Assert.Equal(expectedFirstDir, firstDir);
            Assert.Equal(expectedRest, rest);
        }
    }
}
