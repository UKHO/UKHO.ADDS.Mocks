using UKHO.ADDS.Mocks.Guard;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks.Markdown
{
    /// <summary>
    /// Markdown header.
    /// </summary>
    public class MarkdownHeader : MarkdownTextElement, IMarkdownBlockElement
    {
        private int _level;

        /// <summary>
        /// Gets or sets the header level.
        /// </summary>
        /// <value>The header level.</value>
        public int Level
        {
            get => _level;
            set
            {
                Guard.Guard.Argument(value, nameof(value))
                    .GreaterThan(0)
                    .LessThan(7);
                _level = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownHeader" /> class.
        /// </summary>
        /// <param name="header">The header text.</param>
        /// <param name="level">The header level.</param>
        public MarkdownHeader(string header, int level) : base(header)
        {
            Level = level;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownHeader" /> class.
        /// </summary>
        /// <param name="inlineElement">The header text as markdown inline element.</param>
        /// <param name="level">The header level.</param>
        public MarkdownHeader(MarkdownInlineElement inlineElement, int level) : base(inlineElement)
        {
            Level = level;
        }

        /// <summary>
        /// Returns a string that represents the current markdown header.
        /// </summary>
        /// <returns>A string that represents the current markdown header.</returns>
        public override string ToString()
        {
            return string.Concat(
                new string('#', Level),
                " ",
                Text.Trim(),
                Environment.NewLine);
        }
    }
}
