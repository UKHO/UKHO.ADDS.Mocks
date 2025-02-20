using ADDSMock.Applications.Interactive.Controls;
using ADDSMock.Applications.Interactive.Windows;
using Avalonia.Media;
using ReactiveUI;

namespace ADDSMock.Applications.Interactive.Navigation
{
    internal class NavigationModel : ReactiveObject
    {
        private readonly FrameWindowModel _frameWindow;
        private readonly Type _paneModelType;
        private readonly Type _modelType;
        private IconState _iconState;
        private IImmutableSolidColorBrush _markerBrush;

        public NavigationModel(FrameWindowModel frameWindow, Type paneModelType, Type modelType, string title, string iconName)
        {
            _frameWindow = frameWindow;
            _paneModelType = paneModelType;
            _modelType = modelType;
            Title = title;
            IconName = iconName;

            _iconState = IconState.Normal;
            _markerBrush = Brushes.Transparent;
        }

        public string Title { get; }
        public string IconName { get; }

        public IconState IconState
        {
            get => _iconState;
            set => this.RaiseAndSetIfChanged(ref _iconState, value);
        }

        public IImmutableSolidColorBrush MarkerBrush
        {
            get => _markerBrush;
            set => this.RaiseAndSetIfChanged(ref _markerBrush, value);
        }

        public void Select()
        {
            foreach (var model in _frameWindow.NavigationModels)
            {
                model.Deselect();
            }

            IconState = IconState.Selected;
            MarkerBrush = CommonBrushes.Selected;

            _frameWindow.ShowPane(_paneModelType, _modelType);
        }

        public void Deselect()
        {
            IconState = IconState.Normal;
            MarkerBrush = Brushes.Transparent;
        }
    }
}
