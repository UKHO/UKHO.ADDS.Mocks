using System.Collections.ObjectModel;
using System.Text.Json;
using ADDSMock.Applications.Interactive.Views.Mappings.Messages;
using ADDSMock.Applications.Interactive.Views.Mappings.Models;
using ADDSMock.Domain.Events;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using ReactiveUI;

namespace ADDSMock.Applications.Interactive.Views.Mappings
{
    internal class MappingsModel : ReactiveObject
    {
        private readonly ObservableCollection<EndpointMappingModel> _endpoints;

        private EndpointMappingModel? _selectedEndpoint;
        private TextDocument? _fragmentCode;
        private TextDocument? _outputText;
        private TextMate.Installation? _textMate;
        private Action _setOutputToJsonAction;
        private Action _setOutputToTextAction;

        private bool _showNoView;

        public MappingsModel()
        {
            _endpoints = [];

            MessageBus.Current.Listen<EndpointsSelectedEvent>().Subscribe(x =>
            {
                _endpoints.Clear();

                foreach (var fragment in x.Fragments)
                {
                    _endpoints.Add(fragment);
                }

                if (_endpoints.Any())
                {
                    SelectedEndpoint = _endpoints.First();
                }

                ShowNoView = false;
            });

            MessageBus.Current.Listen<MappingsReloadingEvent>().Subscribe(x =>
            {
                ShowNoView = true;
                _endpoints.Clear();
            });

            _textMate = null;

            ShowNoView = true;
        }

        public bool ShowNoView
        {
            get => _showNoView;
            set => this.RaiseAndSetIfChanged(ref _showNoView, value);
        }

        public IEnumerable<EndpointMappingModel> Endpoints => _endpoints;

        public EndpointMappingModel? SelectedEndpoint
        {
            get => _selectedEndpoint;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedEndpoint, value);

                if (_selectedEndpoint != null)
                {
                    FragmentCode = new TextDocument(_selectedEndpoint.Fragment.Content);

                    if (_selectedEndpoint.IsErrorModel)
                    {
                        _setOutputToTextAction();
                        OutputText = new TextDocument(_selectedEndpoint.Fragment.Error);

                    }
                    else
                    {
                        _setOutputToJsonAction();

                        var mappingJson = JsonSerializer.Serialize(_selectedEndpoint.EndpointMapping!, new JsonSerializerOptions() { WriteIndented = true });
                        OutputText = new TextDocument(mappingJson);
                    }
                }
            }
        }

        public TextDocument? FragmentCode
        {
            get => _fragmentCode;
            set
            {
                this.RaiseAndSetIfChanged(ref _fragmentCode, value);
            }
        }

        public TextDocument? OutputText
        {
            get => _outputText;
            set
            {
                this.RaiseAndSetIfChanged(ref _outputText, value);
            }
        }

        internal void SetEditorActions(Action setOutputToJson, Action setOutputToText)
        {
            _setOutputToJsonAction = setOutputToJson;
            _setOutputToTextAction = setOutputToText;
        }
    }
}
