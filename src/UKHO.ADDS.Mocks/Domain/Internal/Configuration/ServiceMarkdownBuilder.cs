//using System.Text;

//namespace UKHO.ADDS.Mocks.Domain.Internal.Configuration
//{
//    internal class ServiceMarkdownBuilder : IServiceMarkdownBuilder
//    {
//        private readonly StringBuilder _sb = new();

//        public IServiceMarkdownBuilder Append(string text = "")
//        {
//            _sb.Append(text);
//            return this;
//        }

//        public IServiceMarkdownBuilder AppendLine()
//        {
//            _sb.AppendLine();
//            return this;
//        }

//        public IServiceMarkdownBuilder AppendLine(string text)
//        {
//            _sb.AppendLine(text);
//            return this;
//        }


//        public IServiceMarkdownBuilder AppendNewLine()
//        {
//            _sb.Append("  "); // Markdown soft line break
//            _sb.AppendLine();
//            return this;
//        }

//        public IServiceMarkdownBuilder Heading(int level, string text)
//        {
//            level = Math.Clamp(level, 1, 6);
//            return Append($"{new string('#', level)} {text}");
//        }

//        public IServiceMarkdownBuilder Paragraph(string text)
//        {
//            return Append(text).AppendNewLine();
//        }

//        public IServiceMarkdownBuilder Italic(string text)
//        {
//            return Append($"*{text}*");
//        }

//        public IServiceMarkdownBuilder Bold(string text)
//        {
//            return Append($"**{text}**");
//        }

//        public IServiceMarkdownBuilder List(params string[] items)
//        {
//            foreach (var item in items)
//            {
//                Append($"- {item}");
//            }
//            return this;
//        }

//        public IServiceMarkdownBuilder NumberedList(params string[] items)
//        {
//            for (var i = 0; i < items.Length; i++)
//            {
//                Append($"{i + 1}. {items[i]}");
//            }
//            return this;
//        }

//        public IServiceMarkdownBuilder CodeBlock(string code, string language = "")
//        {
//            Append($"```{language}");
//            Append(code);
//            Append("```");
//            return this;
//        }

//        public IServiceMarkdownBuilder Quote(string text)
//        {
//            return Append($"> {text}");
//        }

//        public IServiceMarkdownBuilder HorizontalRule()
//        {
//            return Append("---");
//        }

//        public IServiceMarkdownBuilder Link(string label, string url)
//        {
//            return Append($"[{label}]({url})");
//        }

//        public override string ToString() => _sb.ToString();
//    }

//}
