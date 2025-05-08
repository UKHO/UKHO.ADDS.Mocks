// ReSharper disable once CheckNamespace

namespace UKHO.ADDS.Mocks.Markdown
{
    /// <summary>
    ///     Markdown strong emphasis.
    /// </summary>
    public class MarkdownStrongEmphasis : MarkdownInlineElement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownStrongEmphasis" /> class.
        /// </summary>
        /// <param name="text">The strong emphasis text.</param>
        /// <param name="char">The strong emphasis character. Default is '*'.</param>
        public MarkdownStrongEmphasis(string text, char @char = '*') : base(text) => Char = @char;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownStrongEmphasis" /> class.
        /// </summary>
        /// <param name="inlineElement">The strong emphasis text as markdown inline element.</param>
        /// <param name="char">The strong emphasis character. Default is '*'.</param>
        public MarkdownStrongEmphasis(MarkdownInlineElement inlineElement, char @char = '*') : base(inlineElement) => Char = @char;

        /// <summary>
        ///     Gets or sets the strong emphasis character.
        /// </summary>
        /// <value>Strong emphasis character.</value>
        public char Char { get; }

        /// <summary>
        ///     Returns a string that represents the current markdown strong emphasis.
        /// </summary>
        /// <returns>A string that represents the current markdown strong emphasis.</returns>
        public override string ToString() => $"{Char}{Char}{Text}{Char}{Char}";
    }
}
