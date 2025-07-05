using UKHO.ADDS.Mocks.Domain.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Markdown
{
    public class MarkdownParagraphTest
    {
        [Fact]
        public void TestText() => Assert.Equal("Paragraph", new MarkdownParagraph("Paragraph").Text);

        [Fact]
        public void TestInlineElement()
        {
            var inlineElement = new MarkdownText("Inline element");
            Assert.Equal("Inline element", new MarkdownParagraph(inlineElement).Text);
        }

        [Fact]
        public void TestToString() => Assert.Equal("Paragraph" + Environment.NewLine, new MarkdownParagraph("Paragraph").ToString());
    }
}
