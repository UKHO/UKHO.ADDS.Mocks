using ADDSMock.Applications.Interactive.Controls;
using Avalonia.Media;
using ReactiveUI;

namespace ADDSMock.Applications.Interactive.Views.Mappings.Models
{
    internal class MethodModel : ReactiveObject
    {
        private readonly string _method;

        public MethodModel(string method)
        {
            _method = method;
        }

        public string Method => _method;

        public IImmutableSolidColorBrush TextBrush
        {
            get
            {
                return _method.ToLowerInvariant() switch
                {
                    "get" => CommonBrushes.SelectedAlt,
                    "post" => CommonBrushes.Selected,
                    "put" => CommonBrushes.Warning,
                    "Delete" => CommonBrushes.Error,
                    _ => Brushes.White
                };
            }
        }
    }
}
