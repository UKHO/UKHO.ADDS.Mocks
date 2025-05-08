using UKHO.ADDS.Mocks.Guard;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks.Markdown
{
    /// <summary>
    ///     Markdown table header.
    /// </summary>
    public class MarkdownTableHeader
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTableHeader" /> class.
        /// </summary>
        /// <param name="cells">The cells.</param>
        public MarkdownTableHeader(params MarkdownTableHeaderCell[] cells)
        {
            Guard.Guard.Argument(cells, nameof(cells))
                .NotEmpty(cells => "Table header cells length must be greater that 0.");

            Cells = (MarkdownTableHeaderCell[])cells.Clone();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTableHeader" /> class.
        /// </summary>
        /// <param name="capacity">The header cell capacity.</param>
        public MarkdownTableHeader(int capacity)
        {
            Guard.Guard.Argument(capacity, nameof(capacity))
                .GreaterThan(0, (value, other) => "Table header cells capacity must be greater that 0.");

            Cells = new MarkdownTableHeaderCell[capacity];
        }

        /// <summary>Gets the cells.</summary>
        /// <value>The cells.</value>
        public MarkdownTableHeaderCell[] Cells { get; }

        /// <summary>
        ///     Returns a string that represents the current markdown table header.
        /// </summary>
        /// <returns>A string that represents the current markdown table header.</returns>
        public override string ToString()
        {
            var headerTexts = string.Empty;
            var columnAlignments = string.Empty;

            foreach (var cell in Cells)
            {
                headerTexts = string.Concat(headerTexts, $"| {cell.Text} ");
                columnAlignments = string.Concat(columnAlignments, $"| {cell.ColumnTextAlignment.Print()} ");
            }

            headerTexts = string.Concat(headerTexts, "|");
            columnAlignments = string.Concat(columnAlignments, "|");

            return string.Concat(headerTexts, Environment.NewLine, columnAlignments);
        }
    }
}
