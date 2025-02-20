using ADDSMock.Applications.Interactive.Services;
using ADDSMock.Applications.Interactive.Windows;
using ADDSMock.Domain.Services;
using ADDSMock.Extensions;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using ErrorWindow = ADDSMock.Applications.Interactive.Windows.ErrorWindow;

namespace ADDSMock.Applications.Interactive
{
    internal class InteractiveApplication : Application
    {
        private IServiceProvider? _serviceProvider;

        public override void Initialize()
        {
            Styles.Add(new FluentTheme());
            RequestedThemeVariant = ThemeVariant.Dark;

            var editorStyleUri = new Uri("avares://AvaloniaEdit/Themes/Fluent/AvaloniaEdit.xaml");
            var editorStyleInclude = new StyleInclude(editorStyleUri) { Source = editorStyleUri };

            Styles.Add(editorStyleInclude);

            var serviceCollection = new ServiceCollection();
            var result = serviceCollection.AddInteractive();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var wireMockService = _serviceProvider!.GetRequiredService<IWireMockService>();
                var wireMockStartResult = wireMockService.Start();

                if (wireMockStartResult.IsFailed)
                {
                    var errorWindow = new ErrorWindow();
                    errorWindow.ShowErrorMessage(wireMockStartResult.Error.Message);

                    desktop.MainWindow = errorWindow;
                }
                else
                {
                    desktop.Exit += (sender, e) => { wireMockService.Stop(); };

                    var viewService = _serviceProvider!.GetRequiredService<IViewService>();

                    var model = viewService.GetModel<FrameWindowModel>();
                    var view = viewService.GetWindowFor(model);

                    desktop.MainWindow = view;
                    desktop.MainWindow.Show();
                }
            }
        }
    }
}
