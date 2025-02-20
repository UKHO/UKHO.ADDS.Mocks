using ADDSMock.Applications.Interactive.Controls;
using ADDSMock.Applications.Interactive.Logging;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ADDSMock.Applications.Interactive.Windows
{
    internal partial class FrameWindow : Window
    {
        private bool _showingLog;
        private bool _showingSideBar;

        public FrameWindow()
        {
            InitializeComponent();

            _logEditor.TextArea.TextView.LineTransformers.Add(new ConsoleLogColorizer());

            _showingLog = true;
            _showingSideBar = true;

            _logIcon.State = IconState.Selected;
            _sideBarIcon.State = IconState.Selected;
            _topmostIcon.State = IconState.Normal;

            Loaded += (sender, e) => { ((FrameWindowModel)DataContext!).Initialize(); };
        }

        private void TopmostButtonClick(object? sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
            _topmostIcon.State = Topmost ? IconState.Selected : IconState.Normal;
        }

        private void SideBarButtonClick(object? sender, RoutedEventArgs e)
        {
            _showingSideBar = !_showingSideBar;

            _splitView.IsPaneOpen = _showingSideBar;
            _sideBarIcon.State = _showingSideBar ? IconState.Selected : IconState.Normal;
        }

        private void LogButtonClick(object? sender, RoutedEventArgs e)
        {
            _showingLog = !_showingLog;

            if (_showingLog)
            {
                _logIcon.State = IconState.Selected;

                _logSplitter.IsVisible = true;
                _logGrid.IsVisible = true;

                Grid.SetRowSpan(_contentGrid, 1);
            }
            else
            {
                _logIcon.State = IconState.Normal;

                _logSplitter.IsVisible = false;
                _logGrid.IsVisible = false;

                Grid.SetRowSpan(_contentGrid, 3);
            }
        }
    }
}
