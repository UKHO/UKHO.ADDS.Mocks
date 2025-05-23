﻿using UKHO.ADDS.Mocks.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Internal.Markdown
{
    public class MarkdownTableHeaderTest
    {
        [Fact]
        public void TestInitializeWithCells() => Assert.Single(new MarkdownTableHeader(new MarkdownTableHeaderCell("")).Cells);

        [Fact]
        public void TestInitializeWithCapacity() => Assert.Equal(3, new MarkdownTableHeader(3).Cells.Length);

        [Fact]
        public void TestEmptyCells() => Assert.Throws<ArgumentException>(() => new MarkdownTableHeader());

        [Fact]
        public void TestEmptyCapacity() => Assert.Throws<ArgumentOutOfRangeException>(() => new MarkdownTableHeader(0));

        [Fact]
        public void TestToString()
        {
            var header = new MarkdownTableHeader(new MarkdownTableHeaderCell("Header"), new MarkdownTableHeaderCell("Header"));
            Assert.Equal(
                "| Header | Header |" + Environment.NewLine +
                "| --- | --- |",
                header.ToString()
            );
        }
    }
}
