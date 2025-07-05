using UKHO.ADDS.Mocks.Domain.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Markdown
{
    public class MarkdownDocumentExtensionsTest
    {
        [Fact]
        public void TestAppendHeader()
        {
            var document = new MarkdownDocument().AppendHeader("Header", 1);
            var header = Assert.IsType<MarkdownHeader>(document.ElementAt(0));
            Assert.Equal("Header", header.Text);
        }

        [Fact]
        public void TestAppendHeaderWithInlineElement()
        {
            var document = new MarkdownDocument().AppendHeader(new MarkdownText("Header"), 1);
            var header = Assert.IsType<MarkdownHeader>(document.ElementAt(0));
            Assert.Equal("Header", header.Text);
        }

        [Fact]
        public void TestParagraph()
        {
            var document = new MarkdownDocument().AppendParagraph("Text");
            var paragraph = Assert.IsType<MarkdownParagraph>(document.ElementAt(0));
            Assert.Equal("Text", paragraph.Text);
        }

        [Fact]
        public void TestParagraphWithInlineElement()
        {
            var document = new MarkdownDocument().AppendParagraph(new MarkdownText("Text"));
            var paragraph = Assert.IsType<MarkdownParagraph>(document.ElementAt(0));
            Assert.Equal("Text", paragraph.Text);
        }

        [Fact]
        public void TestHorizontalRule()
        {
            var document = new MarkdownDocument().AppendHorizontalRule();
            Assert.IsType<MarkdownHorizontalRule>(document.ElementAt(0));
        }

        [Fact]
        public void TestHorizontalRuleWithChar()
        {
            var document = new MarkdownDocument().AppendHorizontalRule('_');
            var horizontalRule = Assert.IsType<MarkdownHorizontalRule>(document.ElementAt(0));
            Assert.Equal('_', horizontalRule.Char);
        }

        [Fact]
        public void TestList()
        {
            var document = new MarkdownDocument().AppendList("First", "Second");
            var list = Assert.IsType<MarkdownList>(document.ElementAt(0));
            Assert.Equal(2, list.ListItems.Count);
        }

        [Fact]
        public void TestListWithListItem()
        {
            var document = new MarkdownDocument().AppendList(new MarkdownTextListItem("First"), new MarkdownTextListItem("Second"));
            var list = Assert.IsType<MarkdownList>(document.ElementAt(0));
            Assert.Equal(2, list.ListItems.Count);
        }

        [Fact]
        public void TestListWithListItemEnumerable()
        {
            var document = new MarkdownDocument().AppendList(
                new List<MarkdownTextListItem> { new("First"), new("Second") }
            );
            var list = Assert.IsType<MarkdownList>(document.ElementAt(0));
            Assert.Equal(2, list.ListItems.Count);
        }

        [Fact]
        public void TestListWhithChar()
        {
            var document = new MarkdownDocument().AppendList('*', "First", "Second");
            var list = Assert.IsType<MarkdownList>(document.ElementAt(0));
            Assert.Equal(2, list.ListItems.Count);
            Assert.Equal('*', list.Char);
        }

        [Fact]
        public void TestListWithCharAndListItem()
        {
            var document = new MarkdownDocument().AppendList('*', new MarkdownTextListItem("First"), new MarkdownTextListItem("Second"));
            var list = Assert.IsType<MarkdownList>(document.ElementAt(0));
            Assert.Equal(2, list.ListItems.Count);
            Assert.Equal('*', list.Char);
        }

        [Fact]
        public void TestListWithCharAndListItemEnumerable()
        {
            var document = new MarkdownDocument().AppendList(
                '*',
                new List<MarkdownTextListItem> { new("First"), new("Second") }
            );
            var list = Assert.IsType<MarkdownList>(document.ElementAt(0));
            Assert.Equal(2, list.ListItems.Count);
            Assert.Equal('*', list.Char);
        }

        [Fact]
        public void TestOrderedList()
        {
            var document = new MarkdownDocument().AppendOrderedList("First", "Second");
            var list = Assert.IsType<MarkdownOrderedList>(document.ElementAt(0));
            Assert.Equal(2, list.ListItems.Count);
        }

        [Fact]
        public void TestOrderedListWithListItem()
        {
            var document = new MarkdownDocument().AppendOrderedList(new MarkdownTextListItem("First"), new MarkdownTextListItem("Second"));
            var list = Assert.IsType<MarkdownOrderedList>(document.ElementAt(0));
            Assert.Equal(2, list.ListItems.Count);
        }

        [Fact]
        public void TestOrderedListWithListItemEnumerable()
        {
            var document = new MarkdownDocument().AppendOrderedList(
                new List<MarkdownTextListItem> { new("First"), new("Second") }
            );
            var list = Assert.IsType<MarkdownOrderedList>(document.ElementAt(0));
            Assert.Equal(2, list.ListItems.Count);
        }

        [Fact]
        public void TestCode()
        {
            var document = new MarkdownDocument().AppendCode("csharp", "Console.WriteLine(\"Hello World!\")");
            var code = Assert.IsType<MarkdownCode>(document.ElementAt(0));
            Assert.Equal("Console.WriteLine(\"Hello World!\")", code.Text);
        }

        [Fact]
        public void TestCodeWithInlineElement()
        {
            var document = new MarkdownDocument().AppendCode("csharp", new MarkdownText("Console.WriteLine(\"Hello World!\")"));
            var code = Assert.IsType<MarkdownCode>(document.ElementAt(0));
            Assert.Equal("Console.WriteLine(\"Hello World!\")", code.Text);
        }

        [Fact]
        public void TestBlockquote()
        {
            var document = new MarkdownDocument().AppendBlockquote("Text");
            var blockquote = Assert.IsType<MarkdownBlockquote>(document.ElementAt(0));
            Assert.Equal("Text", blockquote.Text);
        }

        [Fact]
        public void TestBlockquoteWithInlineElement()
        {
            var document = new MarkdownDocument().AppendBlockquote(new MarkdownText("Text"));
            var blockquote = Assert.IsType<MarkdownBlockquote>(document.ElementAt(0));
            Assert.Equal("Text", blockquote.Text);
        }

        [Fact]
        public void TestTable()
        {
            var document = new MarkdownDocument().AppendTable(
                new MarkdownTableHeader(new MarkdownTableHeaderCell("Header"), new MarkdownTableHeaderCell("Header")),
                new MarkdownTableRow[] { new("cell", "cell"), new("cell", "cell") });
            var table = Assert.IsType<MarkdownTable>(document.ElementAt(0));
            Assert.Equal(2, table.ColumnCount);
            Assert.Equal(2, table.RowsCount);
        }
    }
}
