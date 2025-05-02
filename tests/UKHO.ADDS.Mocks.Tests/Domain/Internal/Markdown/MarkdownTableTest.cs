using UKHO.ADDS.Mocks.Markdown;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Internal.Markdown
{
    public class MarkdownTableTest
    {
        [Fact]
        public void TestHeader()
        {
            {
                var table = new MarkdownTable(new MarkdownTableHeader(3));
                Assert.Equal(3, table.Header.Cells.Length);
            }
            {
                var header = new MarkdownTableHeader(
                    new MarkdownTableHeaderCell(""),
                    new MarkdownTableHeaderCell(""),
                    new MarkdownTableHeaderCell("")
                );
                var table = new MarkdownTable(header);
                Assert.Equal(3, table.Header.Cells.Length);
            }
        }

        [Fact]
        public void TestNullHeader() => Assert.Throws<ArgumentNullException>(() => new MarkdownTable(null));

        [Fact]
        public void TestColumnCount()
        {
            var table = new MarkdownTable(new MarkdownTableHeader(3));
            Assert.Equal(3, table.ColumnCount);
        }

        [Fact]
        public void TestRows()
        {
            {
                var table = new MarkdownTable(
                    new MarkdownTableHeader(3),
                    2
                );
                Assert.Equal(0, table.RowsCount);
                Assert.Equal(2, table.RowsCapacity);
            }
            {
                var table = new MarkdownTable(
                    new MarkdownTableHeader(3),
                    new MarkdownTableRow[] { new(3), new(3) }
                );
                Assert.Equal(2, table.RowsCount);
                Assert.Equal(2, table.RowsCapacity);
            }
        }

        [Fact]
        public void TestRowsCapacity()
        {
            var table = new MarkdownTable(new MarkdownTableHeader(3));
            table.RowsCapacity = 3;
            Assert.Equal(3, table.RowsCapacity);
        }

        [Fact]
        public void TestNegativeRowsCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new MarkdownTable(
                    new MarkdownTableHeader(3),
                    -1
                )
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new MarkdownTable(new MarkdownTableHeader(3)).RowsCapacity = -1
            );
        }

        [Fact]
        public void TestInitializeWithInvalidRow() =>
            Assert.Throws<ArgumentException>(
                () => new MarkdownTable(
                    new MarkdownTableHeader(3),
                    new MarkdownTableRow[] { new(2) }
                )
            );

        [Fact]
        public void TestAddRow()
        {
            var table = new MarkdownTable(new MarkdownTableHeader(3));
            table.AddRow(new MarkdownTableRow("A", "B", "C"));

            Assert.Equal(1, table.RowsCount);
        }

        [Fact]
        public void TestAddInvalidRow()
        {
            var table = new MarkdownTable(new MarkdownTableHeader(3));
            Assert.Throws<ArgumentException>(() => table.AddRow(new MarkdownTableRow("A", "B")));
            Assert.Equal(0, table.RowsCount);
        }

        [Fact]
        public void TestAddRowRange()
        {
            var table = new MarkdownTable(new MarkdownTableHeader(3));
            MarkdownTableRow[] rows = { new("A", "B", "C"), new("A", "B", "C") };
            table.AddRowRange(rows);

            Assert.Equal(2, table.RowsCount);
        }

        [Fact]
        public void TestAddInvalidRowRange()
        {
            var table = new MarkdownTable(new MarkdownTableHeader(3));
            MarkdownTableRow[] rows = { new("A", "B"), new("A", "B") };
            Assert.Throws<ArgumentException>(() => table.AddRowRange(rows));
            Assert.Equal(0, table.RowsCount);
        }

        [Fact]
        public void TestGetRowAt()
        {
            var table = new MarkdownTable(new MarkdownTableHeader(3));
            var row = new MarkdownTableRow("A", "B", "C");
            table.AddRow(row);

            Assert.Equal(row, table.GetRowAt(0));
        }

        [Fact]
        public void TestRemoveRowAt()
        {
            var table = new MarkdownTable(new MarkdownTableHeader(3));
            table.AddRow(new MarkdownTableRow("A", "B", "C"));
            table.RemoveRowAt(0);

            Assert.Equal(0, table.RowsCount);
        }

        [Fact]
        public void TestToString()
        {
            var table = new MarkdownTable(
                new MarkdownTableHeader(new MarkdownTableHeaderCell("Header"), new MarkdownTableHeaderCell("Header")),
                new MarkdownTableRow[] { new("cell", "cell"), new("cell", "cell") }
            );
            Assert.Equal(
                "| Header | Header |" + Environment.NewLine +
                "| --- | --- |" + Environment.NewLine +
                "| cell | cell |" + Environment.NewLine +
                "| cell | cell |" + Environment.NewLine,
                table.ToString()
            );
        }
    }
}
