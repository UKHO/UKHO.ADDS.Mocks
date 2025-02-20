using System.Collections.ObjectModel;
using ADDSMock.Applications.Interactive.Views.Mappings.Models;
using ADDSMock.Applications.Interactive.Views.Mappings;
using ADDSMock.Domain.Mappings;
using WireMock.Admin.Mappings;
using System.Text;

namespace ADDSMock.Applications.Interactive.Views.Traffic.Models
{
    internal class EndpointTrafficModel : AbstractTrafficModel
    {
        private readonly MockServiceFragment _fragment;
        private readonly MappingModel? _mappingModel;
        private readonly ObservableCollection<AbstractTrafficModel> _children;

        public EndpointTrafficModel(MockService service, MockServiceFragment fragment, MappingModel? mappingModel)
        {
            _fragment = fragment;
            _mappingModel = mappingModel;
            _children = [];

            IsMethodsVisible = true;
        }

        public override IEnumerable<AbstractTrafficModel> Children => _children;

        public override string Name => (_mappingModel == null) ? "ERROR" : GetModelName();

        public override string Description => "";

        public override IEnumerable<MethodModel> Methods
        {
            get
            {
                if (_mappingModel?.Request.Methods != null)
                {
                    return _mappingModel?.Request.Methods.Select(x => new MethodModel(x)) ?? [];
                }

                return [new MethodModel("UNKNOWN")];
            }
        }

        private string GetModelName()
        {
            if (_mappingModel?.Request.Path is PathModel pathModel)
            {
                var sb = new StringBuilder();

                foreach (var matcher in pathModel.Matchers!)
                {
                    sb.Append($"{matcher.Pattern}");
                }

                return sb.ToString();
            }

            return "Unknown";
        }
    }
}
