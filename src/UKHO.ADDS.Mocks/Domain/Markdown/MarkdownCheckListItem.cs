// ReSharper disable once CheckNamespace

namespace UKHO.ADDS.Mocks.Markdown
{
    /// <summary>
    ///     Markdown check list item.
    /// </summary>
    public class MarkdownCheckListItem : MarkdownTextListItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownCheckListItem" /> class.
        /// </summary>
        /// <param name="checked">The item state.</param>
        /// <param name="text">The item text.</param>
        public MarkdownCheckListItem(bool @checked, string text) : base(text) => Checked = @checked;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkdownCheckListItem" /> class.
        /// </summary>
        /// <param name="checked">The item state.</param>
        /// <param name="element">The item text as markdown inline element.</param>
        public MarkdownCheckListItem(bool @checked, MarkdownInlineElement element) : base(element) => Checked = @checked;

        /// <summary>
        ///     Gets or sets the item state.
        /// </summary>
        /// <value><c>true</c> if the item is checked; otherwise, <c>false</c>. The default is <c>false</c>.</value>
        public bool Checked { get; set; }

        /// <summary>
        ///     Returns a string that represents the current markdown check list item.
        ///     The check list item text is trimed.
        /// </summary>
        /// <returns>A string that represents the current markdown check list item.</returns>
        public override string ToString()
        {
            var value = Checked ? 'x' : ' ';

            return $"[{value}] {base.ToString()}";
        }
    }
}
