// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks.Markdown
{
    /// <summary>
    /// Markdown text.
    /// </summary>
    public class MarkdownText : MarkdownInlineElement
    {
        private readonly ICollection<object> _fragments = new List<object>();

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text or a string that represents the markdown text.</value>
        public new string Text => string.Concat(_fragments.Select(fragment => fragment.ToString()));

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownText" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public MarkdownText(string text) : base(text)
        {
            _fragments.Add(text);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownText" /> class.
        /// </summary>
        /// <param name="inlineElement">The text as markdown inline element.</param>
        public MarkdownText(MarkdownInlineElement inlineElement) : base(inlineElement)
        {
            _fragments.Add(inlineElement);
        }

        /// <summary>
        /// Appends the specified text to this instance.
        /// </summary>
        /// <param name="text">The text to append.</param>
        /// <returns>The markdown text.</returns>
        public MarkdownText Append(string text)
        {
            _fragments.Add(text);

            return this;
        }

        /// <summary>
        /// Appends the specified markdown inline element to this instance.
        /// </summary>
        /// <param name="inlineElement">The markdown inline element to append.</param>
        /// <returns>The markdown text.</returns>
        public MarkdownText Append(MarkdownInlineElement inlineElement)
        {
            _fragments.Add(inlineElement);

            return this;
        }

        /// <summary>
        /// Returns a string that represents the current markdown text.
        /// </summary>
        /// <returns>A string that represents the current markdown text.</returns>
        public override string ToString()
        {
            return Text;
        }
    }
}
