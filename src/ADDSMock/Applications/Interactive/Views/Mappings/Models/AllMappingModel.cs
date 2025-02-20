using System.Collections.ObjectModel;
using ADDSMock.Applications.Interactive.Controls;
using ADDSMock.Domain.Services;
using ReactiveUI;

namespace ADDSMock.Applications.Interactive.Views.Mappings.Models
{
    internal class AllMappingModel : AbstractMappingModel
    {
        private readonly string _baseUrl;
        private readonly ObservableCollection<AbstractMappingModel> _children;

        public AllMappingModel(string baseUrl, IMappingService mappingService)
        {
            _baseUrl = baseUrl;
            _children = [];

            foreach (var service in mappingService.Services)
            {
                _children.Add(new ServiceMappingModel(baseUrl, service));
            }

            IsExpanded = _children.Any();
        }

        public override IEnumerable<AbstractMappingModel> Children => _children;

        public override string Name => "All Services";

        public override string Description => string.Empty;

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
