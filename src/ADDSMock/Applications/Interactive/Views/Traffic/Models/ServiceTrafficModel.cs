using System.Collections.ObjectModel;
using ADDSMock.Applications.Interactive.Views.Mappings.Models;
using ADDSMock.Domain.Mappings;
using AvaloniaEdit.Utils;

namespace ADDSMock.Applications.Interactive.Views.Traffic.Models
{
    internal class ServiceTrafficModel : AbstractTrafficModel
    {
        private readonly string _baseUrl;

        private readonly ObservableCollection<AbstractTrafficModel> _children;
        private readonly MockService _service;

        public ServiceTrafficModel(string baseUrl, MockService service)
        {
            _baseUrl = baseUrl;
            _service = service;

            _children = new ObservableCollection<AbstractTrafficModel>();

            foreach (var fragment in _service.Fragments)
            {
                _children.AddRange(fragment.Mappings.Select(x => new EndpointTrafficModel(service, fragment, x)));
            }

            IsDescriptionVisible = true;
        }

        public override IEnumerable<AbstractTrafficModel> Children => _children;

        public override string Name => _service.ServiceName;

        public override string Description => $"{_baseUrl}{_service.ServicePrefix}/";

        public override IEnumerable<MethodModel> Methods => [];
    }
}
