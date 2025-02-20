using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Path = Avalonia.Controls.Shapes.Path;

namespace ADDSMock.Applications.Interactive.Controls
{
    public class Icon : Control
    {
        public static readonly StyledProperty<string> IconNameProperty =
            AvaloniaProperty.Register<Icon, string>(nameof(IconName), "Help - 02");

        public static readonly StyledProperty<IconState> StateProperty =
            AvaloniaProperty.Register<Icon, IconState>(nameof(State));

        private IImage? _bitmap;

        public string IconName
        {
            get => GetValue(IconNameProperty);

            set => SetValue(IconNameProperty, value);
        }

        public IconState State
        {
            get => GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            var name = change.Property.Name;

            InvalidateMeasure();
            InvalidateArrange();

            switch (name)
            {
                case nameof(IconName):
                case nameof(State):
                    CreateBitmap();
                    break;
            }

            InvalidateVisual();

            base.OnPropertyChanged(change);
        }

        public override void Render(DrawingContext context)
        {
            if (_bitmap != null)
            {
                context.DrawImage(_bitmap, new Rect(0, 0, Bounds.Width, Bounds.Height));
            }
        }

        private void CreateBitmap()
        {
            var pathData = IconPathFactory.GetIconPath(IconName);
            var brush = CreateBrush();

            var width = Bounds.Width;
            var height = Bounds.Height;

            var path = new Path
            {
                Data = StreamGeometry.Parse(pathData),
                Fill = brush,
                Stroke = brush,
                StrokeThickness = 1
            };

            path.Arrange(new Rect(new Size(width, height)));

            var internalImage = new DrawingImage
            {
                Drawing = new GeometryDrawing
                {
                    Geometry = path.Data,
                    Brush = brush,
                }
            };

            var pixelSize = new PixelSize((int)internalImage.Size.Width, (int)internalImage.Size.Height);

            using MemoryStream memoryStream = new();
            using (RenderTargetBitmap bitmap = new(pixelSize, new Vector(96, 96)))
            {
                using (var ctx = bitmap.CreateDrawingContext())
                {
                    internalImage.Drawing.Draw(ctx);
                }

                bitmap.Save(memoryStream);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            _bitmap = new Bitmap(memoryStream);
        }

        private IImmutableBrush CreateBrush()
        {
            IImmutableBrush brush = State switch
            {
                IconState.Normal => CommonBrushes.Normal,
                IconState.Ok => CommonBrushes.SelectedAlt,
                IconState.Disabled => CommonBrushes.Disabled,
                IconState.Selected => CommonBrushes.Selected,
                IconState.Warning => CommonBrushes.Warning,
                IconState.Error => CommonBrushes.Error,
                _ => CommonBrushes.Error
            };

            return brush;
        }
    }
}
