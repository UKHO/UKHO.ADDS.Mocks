using System.Collections.ObjectModel;
using ADDSMock.Applications.Interactive.Views.Traffic.Models;
using ADDSMock.Domain.Events;
using ADDSMock.Domain.Services;
using ReactiveUI;
using WireMock.Admin.Requests;

namespace ADDSMock.Applications.Interactive.Views.Traffic
{
    internal class TrafficPaneModel : ReactiveObject
    {
        private readonly string _baseUrl;
        private readonly IMappingService _mappingService;
        private readonly ILoggingService _loggingService;

        private readonly ObservableCollection<AbstractTrafficModel> _models;

        public TrafficPaneModel(IMappingService mappingService, IWireMockService wireMockService)
        {
            _mappingService = mappingService;
            _baseUrl = wireMockService.BaseUrl;

            _models = [new AllTrafficModel(_baseUrl, _mappingService)];

            MessageBus.Current.Listen<MockServicesUpdatedEvent>().Subscribe(_ =>
            {
                _models.Clear();
                _models.Add(new AllTrafficModel(wireMockService.BaseUrl, _mappingService));
            });
        }

        public IEnumerable<AbstractTrafficModel> Models => _models;
    }
}
