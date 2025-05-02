using UKHO.ADDS.Mocks.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Internal.Markdown
{
    public class MarkdownStrikethroughTest
    {
        [Fact]
        public void TestText()
        {
            Assert.Equal("Strikethrough", new MarkdownStrikethrough("Strikethrough").Text);
        }

        [Fact]
        public void TestInlineElement()
        {
            var inlineElement = new MarkdownText("Inline element");
            Assert.Equal("Inline element", new MarkdownStrikethrough(inlineElement).Text);
        }

        [Fact]
        public void TestToString()
        {
            Assert.Equal("~~Strikethrough~~", new MarkdownStrikethrough("Strikethrough").ToString());
        }
    }
}
