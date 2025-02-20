using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace ADDSMock.Applications.Interactive.Controls
{
    public static class CommonBrushes
    {
        public static IImmutableSolidColorBrush Selected = new ImmutableSolidColorBrush(Color.FromRgb(0x55, 0xaa, 0xff));
        public static IImmutableSolidColorBrush SelectedAlt = new ImmutableSolidColorBrush(Colors.LightGreen);
        public static IImmutableSolidColorBrush Disabled = new ImmutableSolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));
        public static IImmutableSolidColorBrush Normal = new ImmutableSolidColorBrush(Color.FromRgb(0xe0, 0xe0, 0xe0));
        public static IImmutableSolidColorBrush Error = new ImmutableSolidColorBrush(Color.FromRgb(0xff, 0x10, 0x10));
        public static IImmutableSolidColorBrush Warning = new ImmutableSolidColorBrush(Colors.Goldenrod);
    }
}
