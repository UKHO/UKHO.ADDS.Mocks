using System.Text;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using ReactiveUI;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace ADDSMock.Applications.Interactive.Logging
{
    internal class ConsoleLog : ReactiveObject, ILogEventSink
    {
        public const string ConsoleTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        public ConsoleLog()
        {
            LogDocument = new TextDocument();

            var builder = new StringBuilder();
            builder.AppendLine("ADDS Mock Service");
            builder.AppendLine("UK Hydrographic Office 2024");
            builder.AppendLine("");

            LogDocument.Insert(0, builder.ToString());
        }

        public TextDocument LogDocument { get; }

        public void Emit(LogEvent logEvent)
        {
            var sb = new StringWriter();

            var formatter = new MessageTemplateTextFormatter(ConsoleTemplate);
            formatter.Format(logEvent, sb);

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                LogDocument.Insert(LogDocument.TextLength, sb.ToString());
                this.RaisePropertyChanged(nameof(LogDocument));
            });
        }

        public void Clear()
        {
            LogDocument.Text = string.Empty;
        }
    }
}
