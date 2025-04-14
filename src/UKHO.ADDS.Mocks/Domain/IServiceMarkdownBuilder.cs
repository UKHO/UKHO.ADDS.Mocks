namespace UKHO.ADDS.Mocks
{
    public interface IServiceMarkdownBuilder
    {
        IServiceMarkdownBuilder Append(string text);

        IServiceMarkdownBuilder AppendLine(string text = "");

        IServiceMarkdownBuilder Heading(int level, string text);

        IServiceMarkdownBuilder Paragraph(string text);

        IServiceMarkdownBuilder Italic(string text);

        IServiceMarkdownBuilder Bold(string text);

        IServiceMarkdownBuilder List(params string[] items);

        IServiceMarkdownBuilder NumberedList(params string[] items);

        IServiceMarkdownBuilder CodeBlock(string code, string language = "");

        IServiceMarkdownBuilder Quote(string text);

        IServiceMarkdownBuilder HorizontalRule();

        IServiceMarkdownBuilder Link(string label, string url);

        string ToString();
    }
}
