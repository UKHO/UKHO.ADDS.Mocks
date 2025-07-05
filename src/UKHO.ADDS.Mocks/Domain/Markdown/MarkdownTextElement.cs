using UKHO.ADDS.Mocks.Domain.Guard;

namespace UKHO.ADDS.Mocks.Domain.Markdown
{
    /// <summary>
    ///     Markdown text element.
    /// </summary>
    public abstract class MarkdownTextElement
    {
        private MarkdownInlineElement _inlineElement;

        private string _text;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTextElement" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        protected MarkdownTextElement(string text) => Text = text;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownTextElement" /> class.
        /// </summary>
        /// <param name="inlineElement">The text as markdown inline element.</param>
        protected MarkdownTextElement(MarkdownInlineElement inlineElement)
        {
            Guard.Guard.Argument(inlineElement, nameof(inlineElement)).NotNull();

            InlineElement = inlineElement;
        }

        /// <summary>
        ///     Gets or sets the markdown inline element.
        /// </summary>
        /// <value>The markdown inline element.</value>
        protected MarkdownInlineElement InlineElement
        {
            get => _inlineElement;
            set
            {
                if (value != null)
                {
                    Text = null;
                }

                _inlineElement = value;
            }
        }

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>The text or a string that represents the markdown inline element.</value>
        public string Text
        {
            get
            {
                if (_text == null && InlineElement != null)
                {
                    return InlineElement.ToString();
                }

                return _text;
            }
            set
            {
                if (value != null)
                {
                    InlineElement = null;
                }

                _text = value;
            }
        }
    }
}
