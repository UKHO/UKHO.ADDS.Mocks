using System.Collections.ObjectModel;
using ADDSMock.Applications.Interactive.Views.Mappings.Models;
using ADDSMock.Domain.Services;

namespace ADDSMock.Applications.Interactive.Views.Traffic.Models
{
    internal class AllTrafficModel : AbstractTrafficModel
    {
        private readonly string _baseUrl;
        private readonly ObservableCollection<AbstractTrafficModel> _children;

        public AllTrafficModel(string baseUrl, IMappingService mappingService)
        {
            _baseUrl = baseUrl;
            _children = [];

            foreach (var service in mappingService.Services)
            {
                _children.Add(new ServiceTrafficModel(baseUrl, service));
            }

            IsExpanded = _children.Any();
        }

        public override IEnumerable<AbstractTrafficModel> Children => _children;

        public override string Name => "All Services";

        public override string Description => string.Empty;
        public override IEnumerable<MethodModel> Methods => [];
    }
}
