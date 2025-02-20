using System.Collections.ObjectModel;
using ADDSMock.Applications.Interactive.Logging;
using ADDSMock.Applications.Interactive.Navigation;
using ADDSMock.Applications.Interactive.Services;
using ADDSMock.Applications.Interactive.Views.Explorer;
using ADDSMock.Applications.Interactive.Views.Mappings;
using ADDSMock.Applications.Interactive.Views.Traffic;
using ADDSMock.Domain.Services;
using Avalonia.Controls;
using ReactiveUI;
using Serilog;

namespace ADDSMock.Applications.Interactive.Windows
{
    internal class FrameWindowModel : ReactiveObject
    {
        private readonly ConsoleLog _consoleLog;
        private readonly IMappingService _mappingService;

        private readonly ObservableCollection<NavigationModel> _navigationModels;
        private readonly IViewService _viewService;
        private readonly IWireMockService _wireMockService;

        private Control? _paneControl;
        private Control? _viewControl;
        private string _title;

        public FrameWindowModel(IWireMockService wireMockService, IViewService viewService, IMappingService mappingService, ConsoleLog consoleLog)
        {
            _wireMockService = wireMockService;
            _viewService = viewService;
            _mappingService = mappingService;
            _consoleLog = consoleLog;
            _title = $"ADDS MOCK [{wireMockService.BaseUrl}]";

            _paneControl = null;
            _viewControl = null;

            _navigationModels =
            [
                new NavigationModel(this, typeof(MappingsPaneModel), typeof(MappingsModel), "Mappings", "Imports-WF"),
                new NavigationModel(this, typeof(TrafficPaneModel), typeof(TrafficModel),"Traffic", "Webpage Sync-WF"),
                new NavigationModel(this, typeof(ExplorerPaneModel), typeof(ExplorerModel),"Explorer", "Search-WF")
            ];

            _navigationModels.First().Select();
        }

        public IEnumerable<NavigationModel> NavigationModels => _navigationModels;

        public ConsoleLog ConsoleLog => _consoleLog;

        public void ClearLog()
        {
            _consoleLog.Clear();
        }

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public Control? PaneControl
        {
            get => _paneControl;
            set => this.RaiseAndSetIfChanged(ref _paneControl, value);
        }

        public Control? ViewControl
        {
            get => _viewControl;
            set => this.RaiseAndSetIfChanged(ref _viewControl, value);
        }

        public void StartWireMock() => _wireMockService.Start();

        public void StopWireMock() => _wireMockService.Stop();

        public void ShowPane(Type paneModelType, Type viewModelType)
        {
            var model = _viewService.GetModel(paneModelType);
            PaneControl = _viewService.GetViewFor(model);

            var viewModel = _viewService.GetModel(viewModelType);
            ViewControl = _viewService.GetViewFor(viewModel);
        }

        public async Task Initialize()
        {
            var mappingResult = await _mappingService.ReadMappingsAsync();

            if (mappingResult.IsFailed)
            {
                Log.Error(mappingResult.Error.Message);
            }
            else
            {
                await _mappingService.ExecuteMappingsAsync(_wireMockService);
            }
        }
    }
}
