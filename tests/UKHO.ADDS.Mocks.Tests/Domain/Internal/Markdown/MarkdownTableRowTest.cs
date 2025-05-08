using UKHO.ADDS.Mocks.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Internal.Markdown
{
    public class MarkdownTableRowTest
    {
        [Fact]
        public void TestIEnumerableInlineElement() => Assert.Single(new MarkdownTableRow(new List<MarkdownInlineElement> { new MarkdownText("Inline Element") }).Cells);

        [Fact]
        public void TestParamsInlineElement() => Assert.Single(new MarkdownTableRow(new MarkdownText("Inline Element")).Cells);

        [Fact]
        public void TestIEnumerableString() => Assert.Single(new MarkdownTableRow(new List<string> { "Text" }).Cells);

        [Fact]
        public void TestParamsString() => Assert.Single(new MarkdownTableRow("Text").Cells);

        [Fact]
        public void TestToString() => Assert.Equal("| A | B | C |", new MarkdownTableRow("A", "B", "C").ToString());
    }
}
