using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace ADDSMock.Applications.Interactive.Logging
{
    internal class ConsoleLogColorizer : DocumentColorizingTransformer
    {
        protected override void ColorizeLine(DocumentLine line)
        {
            var text = CurrentContext.Document.GetText(line);
            
            if (text.Contains("[INF]"))
            {
                ChangeLinePart(line.Offset, line.EndOffset, element => ApplyChanges(element, Brushes.SkyBlue));
            }
            else if (text.Contains("[WRN]"))
            {
                ChangeLinePart(line.Offset, line.EndOffset, element => ApplyChanges(element, Brushes.Goldenrod));
            }
            else if (text.Contains("[ERR]"))
            {
                ChangeLinePart(line.Offset, line.EndOffset, element => ApplyChanges(element, Brushes.Red));
            }
            else if (text.Contains(": error "))
            {
                ChangeLinePart(line.Offset, line.EndOffset, element => ApplyChanges(element, Brushes.Red));
            }
            else if (text.Contains("[FTL]"))
            {
                ChangeLinePart(line.Offset, line.EndOffset, element => ApplyChanges(element, Brushes.Red));
            }
            else if (text.Contains("[VRB]"))
            {
                ChangeLinePart(line.Offset, line.EndOffset, element => ApplyChanges(element, Brushes.SkyBlue));
            }
            else if (text.Contains("[DBG]"))
            {
                ChangeLinePart(line.Offset, line.EndOffset, element => ApplyChanges(element, Brushes.SkyBlue));
            }
            else if (text.Contains("ADDS Mock Service") || text.Contains("UK Hydrographic"))
            {
                ChangeLinePart(line.Offset, line.EndOffset, element => ApplyChanges(element, Brushes.SkyBlue));
            }
            else
            {
                ChangeLinePart(line.Offset, line.EndOffset, element => ApplyChanges(element, Brushes.WhiteSmoke));
            }
        }

        private void ApplyChanges(VisualLineElement element, IImmutableSolidColorBrush brush) => element.TextRunProperties.SetForegroundBrush(brush);
    }
}
