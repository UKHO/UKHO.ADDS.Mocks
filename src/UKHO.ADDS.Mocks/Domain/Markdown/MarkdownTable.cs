using System.Text;
using UKHO.ADDS.Mocks.Guard;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks.Markdown
{
    /// <summary>
    ///     Markdown table.
    /// </summary>
    public class MarkdownTable : IMarkdownBlockElement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTable" /> class.
        /// </summary>
        /// <param name="header">The header.</param>
        public MarkdownTable(MarkdownTableHeader header)
        {
            Guard.Guard.Argument(header, nameof(header))
                .NotNull();

            Header = header;
            Rows = new List<MarkdownTableRow>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTable" /> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="rows">The rows.</param>
        public MarkdownTable(MarkdownTableHeader header, IEnumerable<MarkdownTableRow> rows)
        {
            Guard.Guard.Argument(header, nameof(header))
                .NotNull();

            Header = header;

            foreach (var row in rows)
            {
                CheckRow(row);
            }

            Rows = new List<MarkdownTableRow>(rows);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTable" /> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="capacity">The row capacity.</param>
        public MarkdownTable(MarkdownTableHeader header, int capacity) : this(header)
        {
            Guard.Guard.Argument(header, nameof(header))
                .NotNull();
            CheckRowsCapacity(capacity);

            Header = header;
            Rows = new List<MarkdownTableRow>(capacity);
        }

        /// <summary>Gets the header.</summary>
        /// <value>The header.</value>
        public MarkdownTableHeader Header { get; }

        /// <summary>Gets the rows.</summary>
        /// <value>The rows.</value>
        private List<MarkdownTableRow> Rows { get; }

        /// <summary>
        ///     Gets the number of columns.
        /// </summary>
        public int ColumnCount => Header.Cells.Length;

        /// <summary>
        ///     Gets the number of rows.
        /// </summary>
        public int RowsCount => Rows.Count;

        /// <summary>
        ///     Gets or sets the rows capacity.
        /// </summary>
        /// <value>The rows capacity.</value>
        public int RowsCapacity
        {
            get => Rows.Capacity;
            set
            {
                CheckRowsCapacity(value);
                Rows.Capacity = value;
            }
        }

        /// <summary>
        ///     Returns a string that represents the current markdown table.
        /// </summary>
        /// <returns>A string that represents the current markdown table.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(Header.ToString());

            foreach (var row in Rows)
            {
                sb.AppendLine(row.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Adds the specific row at the end of the table.
        /// </summary>
        /// <param name="row">The row to be added.</param>
        public void AddRow(MarkdownTableRow row)
        {
            CheckRow(row);
            Rows.Add(row);
        }

        /// <summary>
        ///     Adds the specific rows at the end of the table.
        /// </summary>
        /// <param name="rows">The rows to be added.</param>
        public void AddRowRange(IEnumerable<MarkdownTableRow> rows)
        {
            foreach (var row in rows)
            {
                CheckRow(row);
            }

            Rows.AddRange(rows);
        }

        /// <summary>
        ///     Gets the row at the specific index.
        /// </summary>
        /// <param name="index">The row index.</param>
        /// <returns>The row</returns>
        public MarkdownTableRow GetRowAt(int index) => Rows.ElementAt(index);

        /// <summary>
        ///     Remove the row at the specific index.
        /// </summary>
        /// <param name="index">The row index.</param>
        public void RemoveRowAt(int index) => Rows.RemoveAt(index);

        private void CheckRowsCapacity(int capacity) =>
            Guard.Guard.Argument(capacity, nameof(capacity))
                .GreaterThan(-1, (value, other) => "Table rows capacity must be greater that or equal to 0.");

        private void CheckRow(MarkdownTableRow row)
        {
            Guard.Guard.Argument(row, nameof(row)).NotNull();

            if (row.Cells.Length != Header.Cells.Length)
            {
                throw new ArgumentException("Rows must have the same number of cells as headers.");
            }
        }
    }
}
