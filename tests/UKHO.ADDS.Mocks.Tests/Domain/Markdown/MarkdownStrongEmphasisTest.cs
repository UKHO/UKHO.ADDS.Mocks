using UKHO.ADDS.Mocks.Domain.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Markdown
{
    public class MarkdownStringEmphasisTest
    {
        [Fact]
        public void TestText() => Assert.Equal("Strong emphasis", new MarkdownStrongEmphasis("Strong emphasis").Text);

        [Fact]
        public void TestInlineElement()
        {
            var inlineElement = new MarkdownText("Inline element");
            Assert.Equal("Inline element", new MarkdownStrongEmphasis(inlineElement).Text);
        }

        [Fact]
        public void TestToString() => Assert.Equal("**Strong emphasis**", new MarkdownStrongEmphasis("Strong emphasis").ToString());
    }
}
