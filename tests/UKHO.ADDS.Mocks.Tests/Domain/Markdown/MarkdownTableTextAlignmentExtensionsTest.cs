using UKHO.ADDS.Mocks.Domain.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Markdown
{
    public class MarkdownTableTextAlignmentExtensionsTest
    {
        [Fact]
        public void TestDefaultToString() => Assert.Equal("---", MarkdownTableTextAlignment.Default.Print());

        [Fact]
        public void TestLeftToString() => Assert.Equal(":--", MarkdownTableTextAlignment.Left.Print());

        [Fact]
        public void TestCenterToString() => Assert.Equal(":-:", MarkdownTableTextAlignment.Center.Print());

        [Fact]
        public void TestRightToString() => Assert.Equal("--:", MarkdownTableTextAlignment.Right.Print());
    }
}
