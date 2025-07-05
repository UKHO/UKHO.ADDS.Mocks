using UKHO.ADDS.Mocks.Domain.Guard;

namespace UKHO.ADDS.Mocks.Domain.Markdown
{
    /// <summary>
    ///     Markdown horizontal rule.
    /// </summary>
    public class MarkdownHorizontalRule : IMarkdownBlockElement
    {
        private char _char;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownHorizontalRule" /> class.
        /// </summary>
        public MarkdownHorizontalRule() => Char = '-';

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownHorizontalRule" /> class.
        /// </summary>
        /// <param name="char">The horizontal rule character.</param>
        public MarkdownHorizontalRule(char @char) => Char = @char;

        /// <summary>
        ///     Gets or sets the horizontal rule character.
        /// </summary>
        /// <value>Horizontal rule character.</value>
        public char Char
        {
            get => _char;
            set
            {
                Guard.Guard.Argument(value, nameof(value))
                    .In('-', '*', '_');
                _char = value;
            }
        }

        /// <summary>
        ///     Returns a string that represents the current markdown horizontal rule.
        /// </summary>
        /// <returns>A string that represents the current markdown horizontal rule.</returns>
        public override string ToString() => string.Concat(Char, Char, Char, Environment.NewLine);
    }
}
