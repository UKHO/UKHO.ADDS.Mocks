using UKHO.ADDS.Mocks.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Internal.Markdown
{
    public class MarkdownBlockquoteTest
    {
        [Fact]
        public void TestText() => Assert.Equal("Blockquote", new MarkdownBlockquote("Blockquote").Text);

        [Fact]
        public void TestInlineElement()
        {
            var inlineElement = new MarkdownText("Inline element");
            Assert.Equal("Inline element", new MarkdownBlockquote(inlineElement).Text);
        }

        [Fact]
        public void TestToString() => Assert.Equal($"> Blockquote{Environment.NewLine}", new MarkdownBlockquote("Blockquote").ToString());
    }
}
