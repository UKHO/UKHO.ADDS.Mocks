using Avalonia.Controls;
using Avalonia.Threading;

namespace ADDSMock.Applications.Interactive.Windows
{
    internal partial class ErrorWindow : Window
    {
        public ErrorWindow()
        {
            InitializeComponent();
        }

        public void ShowErrorMessage(string message)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                _errorMessage.Text = message;
            });
        }
    }
}
