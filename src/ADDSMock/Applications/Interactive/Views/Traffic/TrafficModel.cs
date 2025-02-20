using ADDSMock.Domain.Services;
using ADDSMock.Domain.Traffic;
using ReactiveUI;
using WireMock.Admin.Requests;

namespace ADDSMock.Applications.Interactive.Views.Traffic
{
    internal class TrafficModel : ReactiveObject
    {
        private readonly ILoggingService _loggingService;
        private RequestResponseModel? _selectedRequestResponse;
        private bool _showNoView;

        public TrafficModel(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            ShowNoView = !_loggingService.TrafficLogs.Any();

            _loggingService.TrafficLogs.CollectionChanged += (sender, args) =>
            {
                ShowNoView = !_loggingService.TrafficLogs.Any();
            };
        }

        public IEnumerable<RequestResponseModel> TrafficLogs => _loggingService.TrafficLogs;

        public RequestResponseModel? SelectedRequestResponse
        {
            get => _selectedRequestResponse;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedRequestResponse, value);
            }
        }

        public bool ShowNoView
        {
            get => _showNoView;
            set => this.RaiseAndSetIfChanged(ref _showNoView, value);
        }
    }
}
