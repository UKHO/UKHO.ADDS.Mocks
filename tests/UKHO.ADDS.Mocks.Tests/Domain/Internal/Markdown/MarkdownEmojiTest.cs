using UKHO.ADDS.Mocks.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Internal.Markdown
{
    public class MarkdownEmojiTest
    {
        [Fact]
        public void TestText()
        {
            Assert.Equal("thumbsup", new MarkdownEmoji("thumbsup").Text);
        }

        [Fact]
        public void TestToString()
        {
            Assert.Equal(":thumbsup:", new MarkdownEmoji("thumbsup").ToString());
        }
    }
}
