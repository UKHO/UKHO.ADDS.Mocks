﻿using UKHO.ADDS.Mocks.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Internal.Markdown
{
    public class MarkdownImageTest
    {
        [Fact]
        public void TestText() => Assert.Equal("image", new MarkdownImage("image", "url").Text);

        [Fact]
        public void TestUrl() => Assert.Equal("url", new MarkdownImage("image", "url").Url);

        [Fact]
        public void TestToString() => Assert.Equal("![image](url)", new MarkdownImage("image", "url").ToString());
    }
}
