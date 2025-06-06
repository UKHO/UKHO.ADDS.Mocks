using UKHO.ADDS.Mocks.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Internal.Markdown
{
    public class MarkdownTextListTest
    {
        [Fact]
        public void TestEmptyConstructor()
        {
            var list = new MarkdownList();

            Assert.Empty(list.ListItems);
            Assert.Equal('-', list.Char);
            Assert.Equal(string.Empty, list.ToString());
        }

        [Fact]
        public void TestConstructorWithChar()
        {
            Assert.Equal('-', new MarkdownList('-').Char);
            Assert.Equal('*', new MarkdownList('*').Char);
        }

        [Fact]
        public void TestConstructorInvalidChar() => Assert.Throws<ArgumentException>(() => new MarkdownList('a'));

        [Fact]
        public void TestConstructorWithListItemsArray()
        {
            var list = new MarkdownList(
                new MarkdownTextListItem("One"),
                new MarkdownTextListItem("Two")
            );

            Assert.Equal('-', list.Char);
            Assert.Equal(2, list.ListItems.Count);
        }

        [Fact]
        public void TestConstructorWithListItemsEnumerable()
        {
            var list = new MarkdownList(
                new List<MarkdownTextListItem> { new("One"), new("Two") }
            );

            Assert.Equal('-', list.Char);
            Assert.Equal(2, list.ListItems.Count);
        }

        [Fact]
        public void TestConstructorWithStringArray()
        {
            var list = new MarkdownList("One", "Two");

            Assert.Equal('-', list.Char);
            Assert.Equal(2, list.ListItems.Count);
        }

        [Fact]
        public void TestConstructorWithCharAndListItemsArray()
        {
            {
                var list = new MarkdownList(
                    '-',
                    new MarkdownTextListItem("One"),
                    new MarkdownTextListItem("Two")
                );

                Assert.Equal('-', list.Char);
                Assert.Equal(2, list.ListItems.Count);
            }
            {
                var list = new MarkdownList(
                    '*',
                    new MarkdownTextListItem("One"),
                    new MarkdownTextListItem("Two")
                );

                Assert.Equal('*', list.Char);
                Assert.Equal(2, list.ListItems.Count);
            }
        }

        [Fact]
        public void TestConstructorInvalidCharAndListItemsArray() =>
            Assert.Throws<ArgumentException>(
                () => new MarkdownList(
                    'a',
                    new MarkdownTextListItem("One"),
                    new MarkdownTextListItem("Two")
                )
            );

        [Fact]
        public void TestConstructorWithCharAndListItemsEnumerable()
        {
            {
                var list = new MarkdownList(
                    '-',
                    new List<MarkdownTextListItem> { new("One"), new("Two") }
                );

                Assert.Equal('-', list.Char);
                Assert.Equal(2, list.ListItems.Count);
            }
            {
                var list = new MarkdownList(
                    '*',
                    new List<MarkdownTextListItem> { new("One"), new("Two") }
                );

                Assert.Equal('*', list.Char);
                Assert.Equal(2, list.ListItems.Count);
            }
        }

        [Fact]
        public void TestConstructorInvalidCharAndListItemsEnumerable() =>
            Assert.Throws<ArgumentException>(
                () => new MarkdownList(
                    'a',
                    new List<MarkdownTextListItem> { new("One"), new("Two") }
                )
            );

        [Fact]
        public void TestConstructorWithCharAndStringArray()
        {
            {
                var list = new MarkdownList('-', "One", "Two");

                Assert.Equal('-', list.Char);
                Assert.Equal(2, list.ListItems.Count);
            }
            {
                var list = new MarkdownList('*', "One", "Two");

                Assert.Equal('*', list.Char);
                Assert.Equal(2, list.ListItems.Count);
            }
        }

        [Fact]
        public void TestConstructorInvalidCharAndStringArray() => Assert.Throws<ArgumentException>(() => new MarkdownList('a', "One", "Two"));

        [Fact]
        public void TestAddStringItem()
        {
            var list = new MarkdownList();
            list.AddItem("item");
            Assert.Equal("item", (list.ListItems.First() as MarkdownTextListItem).Text);
        }

        [Fact]
        public void TestAddInlineElementItem()
        {
            var list = new MarkdownList();
            list.AddItem(new MarkdownText("item"));
            Assert.Equal("item", (list.ListItems.First() as MarkdownTextListItem).Text);
        }

        [Fact]
        public void TestToString()
        {
            var list = new MarkdownList(
                new MarkdownTextListItem("One"),
                new MarkdownTextListItem("Two"),
                new MarkdownList(
                    new MarkdownTextListItem("First"),
                    new MarkdownTextListItem("Second"),
                    new MarkdownList(
                        new MarkdownTextListItem("First"),
                        new MarkdownTextListItem("Second")
                    )
                )
            );

            Assert.Equal(
                "- One" + Environment.NewLine +
                "- Two" + Environment.NewLine +
                "  - First" + Environment.NewLine +
                "  - Second" + Environment.NewLine +
                "    - First" + Environment.NewLine +
                "    - Second" + Environment.NewLine, list.ToString());
        }
    }
}
