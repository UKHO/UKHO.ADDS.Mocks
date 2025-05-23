// ReSharper disable once CheckNamespace

namespace UKHO.ADDS.Mocks.Markdown
{
    /// <summary>
    ///     Markdown code.
    /// </summary>
    public class MarkdownCode : MarkdownTextElement, IMarkdownBlockElement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownCode" /> class.
        /// </summary>
        /// <param name="language">The code language.</param>
        /// <param name="code">The code.</param>
        public MarkdownCode(string language, string code) : base(code) => Language = language;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownCode" /> class.
        /// </summary>
        /// <param name="language">The code language.</param>
        /// <param name="inlineElement">The code as markdown inline element.</param>
        public MarkdownCode(string language, MarkdownInlineElement inlineElement) : base(inlineElement) => Language = language;

        /// <summary>
        ///     Gets or sets the code language.
        /// </summary>
        /// <value>Code language.</value>
        public string Language { get; set; }

        /// <summary>
        ///     Returns a string that represents the current markdown code.
        /// </summary>
        /// <returns>A string that represents the current markdown code.</returns>
        public override string ToString() =>
            string.Join(Environment.NewLine,
                $"```{Language}",
                Text,
                $"```{Environment.NewLine}");
    }
}
