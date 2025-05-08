using UKHO.ADDS.Mocks.Guard;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks.Markdown
{
    /// <summary>
    ///     Markdown table row.
    /// </summary>
    public class MarkdownTableRow
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTableRow" /> class.
        /// </summary>
        /// <param name="cells">The cells.</param>
        public MarkdownTableRow(IEnumerable<MarkdownInlineElement> cells)
        {
            Guard.Guard.Argument(cells, nameof(cells)).NotNull();

            Cells = cells.ToArray();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTableRow" /> class.
        /// </summary>
        /// <param name="cells">The cells.</param>
        public MarkdownTableRow(params MarkdownInlineElement[] cells)
        {
            Guard.Guard.Argument(cells, nameof(cells)).NotNull();

            Cells = (MarkdownInlineElement[])cells.Clone();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTableRow" /> class.
        /// </summary>
        /// <param name="cells">The cells.</param>
        public MarkdownTableRow(IEnumerable<string> cells)
        {
            Guard.Guard.Argument(cells, nameof(cells)).NotNull();

            Cells = cells.Select(cell => new MarkdownText(cell)).ToArray();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTableRow" /> class.
        /// </summary>
        /// <param name="cells">The cells.</param>
        public MarkdownTableRow(params string[] cells)
        {
            Guard.Guard.Argument(cells, nameof(cells)).NotNull();

            Cells = cells.Select(cell => new MarkdownText(cell)).ToArray();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTableRow" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public MarkdownTableRow(int capacity)
        {
            Guard.Guard.Argument(capacity, nameof(capacity)).Positive();

            Cells = new MarkdownInlineElement[capacity];
        }

        /// <summary>Gets the cells.</summary>
        /// <value>The cells.</value>
        public MarkdownInlineElement[] Cells { get; }

        /// <summary>
        ///     Returns a string that represents the current markdown table row.
        /// </summary>
        /// <returns>A string that represents the current markdown table row.</returns>
        public override string ToString() => $"{string.Concat(Cells.Select(c => $"| {c} "))}|";
    }
}
