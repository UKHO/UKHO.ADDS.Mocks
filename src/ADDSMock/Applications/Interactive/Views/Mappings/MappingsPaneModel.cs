using ADDSMock.Applications.Interactive.Views.Mappings.Models;
using ADDSMock.Domain.Services;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using ADDSMock.Applications.Interactive.Views.Mappings.Messages;
using ADDSMock.Domain.Events;
using ReactiveUI;

namespace ADDSMock.Applications.Interactive.Views.Mappings
{
    internal class MappingsPaneModel : ReactiveObject
    {
        private readonly IMappingService _mappingService;
        private AbstractMappingModel? _selectedModel;

        private readonly ReactiveCommand<Unit, Task> _reloadCommand;

        private readonly ObservableCollection<AbstractMappingModel> _models;

        public MappingsPaneModel(IMappingService mappingService, IWireMockService wireMockService)
        {
            _mappingService = mappingService;
            var baseUrl = wireMockService.BaseUrl;

            _models = [new AllMappingModel(baseUrl, mappingService)];
            SelectedModel = _models.First();

            MessageBus.Current.Listen<MockServicesUpdatedEvent>().Subscribe(_ =>
            {
                _models.Clear();
                _models.Add(new AllMappingModel(wireMockService.BaseUrl, mappingService));

                SelectedModel = _models.First();
            });

            _reloadCommand = ReactiveCommand.Create(async () =>
            {
                _models.Clear();
                await _mappingService.ReloadMappingsAsync(wireMockService);
            });
        }

        public IEnumerable<AbstractMappingModel> Models => _models;

        public ICommand ReloadCommand => _reloadCommand;

        public AbstractMappingModel? SelectedModel
        {
            get => _selectedModel;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedModel, value);

                if (_selectedModel != null)
                {
                    var fragments = _selectedModel.GetEndpointList();

                    MessageBus.Current.SendMessage(new EndpointsSelectedEvent(fragments));
                }
            }
        }
    }
}
