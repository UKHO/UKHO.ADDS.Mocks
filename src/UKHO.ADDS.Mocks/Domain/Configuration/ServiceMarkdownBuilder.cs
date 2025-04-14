using System.Text;

namespace UKHO.ADDS.Mocks.Domain.Configuration
{
    internal class ServiceMarkdownBuilder : IServiceMarkdownBuilder
    {
        private readonly StringBuilder _sb = new();

        public IServiceMarkdownBuilder Append(string text)
        {
            _sb.Append(text);
            return this;
        }

        public IServiceMarkdownBuilder AppendLine(string text = "")
        {
            _sb.AppendLine(text);
            return this;
        }

        public IServiceMarkdownBuilder Heading(int level, string text)
        {
            level = Math.Clamp(level, 1, 6);
            return AppendLine($"{new string('#', level)} {text}");
        }

        public IServiceMarkdownBuilder Paragraph(string text)
        {
            return AppendLine(text).AppendLine();
        }

        public IServiceMarkdownBuilder Italic(string text)
        {
            return Append($"*{text}*");
        }

        public IServiceMarkdownBuilder Bold(string text)
        {
            return Append($"**{text}**");
        }

        public IServiceMarkdownBuilder List(params string[] items)
        {
            foreach (var item in items)
            {
                AppendLine($"- {item}");
            }
            return this;
        }

        public IServiceMarkdownBuilder NumberedList(params string[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                AppendLine($"{i + 1}. {items[i]}");
            }
            return this;
        }

        public IServiceMarkdownBuilder CodeBlock(string code, string language = "")
        {
            AppendLine($"```{language}");
            AppendLine(code);
            AppendLine("```");
            return this;
        }

        public IServiceMarkdownBuilder Quote(string text)
        {
            return AppendLine($"> {text}");
        }

        public IServiceMarkdownBuilder HorizontalRule()
        {
            return AppendLine("---");
        }

        public IServiceMarkdownBuilder Link(string label, string url)
        {
            return Append($"[{label}]({url})");
        }

        public override string ToString() => _sb.ToString();
    }

}
