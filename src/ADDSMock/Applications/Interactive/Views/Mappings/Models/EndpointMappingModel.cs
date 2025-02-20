using ADDSMock.Applications.Interactive.Controls;
using ADDSMock.Domain.Mappings;
using Avalonia.Media;
using System.Collections.ObjectModel;
using System.Text;
using WireMock.Admin.Mappings;

namespace ADDSMock.Applications.Interactive.Views.Mappings.Models
{
    internal class EndpointMappingModel : AbstractMappingModel
    {
        private readonly MockServiceFragment _fragment;
        private readonly MappingModel? _mappingModel;

        private readonly ObservableCollection<AbstractMappingModel> _children;

        private readonly string _description;

        public EndpointMappingModel(MockServiceFragment fragment, MappingModel? mappingModel)
        {
            _fragment = fragment;
            _mappingModel = mappingModel;
            _children = [];
            _description = "";
        }

        public override IEnumerable<AbstractMappingModel> Children => _children;

        public override string Name => _fragment.FragmentName;
        public override string Description => _description;
        public override bool HasErrors => _fragment.HasError;

        public IImmutableSolidColorBrush TextBrush => HasErrors ? CommonBrushes.Error : Brushes.WhiteSmoke;

        public bool IsErrorModel => _mappingModel == null;

        public override IEnumerable<EndpointMappingModel> GetEndpointList() => [this];

        public MockServiceFragment Fragment => _fragment;

        public MappingModel? EndpointMapping => _mappingModel;

        public string EndpointName => (_mappingModel == null) ? "ERROR" : GetModelName();

        public IEnumerable<MethodModel> Methods
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
