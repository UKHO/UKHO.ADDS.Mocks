using System.Collections.ObjectModel;
using ADDSMock.Domain.Mappings;

namespace ADDSMock.Applications.Interactive.Views.Mappings.Models
{
    internal class ServiceMappingModel : AbstractMappingModel
    {
        private readonly string _baseUrl;
        private readonly MockService _service;

        readonly ObservableCollection<AbstractMappingModel> _children;

        public ServiceMappingModel(string baseUrl, MockService service)
        {
            _baseUrl = baseUrl;
            _service = service;

            _children = new ObservableCollection<AbstractMappingModel>(_service.Fragments.Select(x => new FragmentMappingModel(_service, x)));

            IsDescriptionVisible = true;
        }

        public override IEnumerable<AbstractMappingModel> Children => _children;
        public override string Name => _service.ServiceName;
        public override string Description => $"{_baseUrl}{_service.ServicePrefix}/";

        public override bool HasErrors
        {
            get
            {
                return _children.Any(x => x.HasErrors);
            }
        }

        public override IEnumerable<EndpointMappingModel> GetEndpointList() => _children.SelectMany(x => x.GetEndpointList());
    }
}
