using System.Collections.ObjectModel;
using ADDSMock.Applications.Interactive.Controls;
using ADDSMock.Domain.Mappings;
using Avalonia.Media;

namespace ADDSMock.Applications.Interactive.Views.Mappings.Models
{
    internal class FragmentMappingModel : AbstractMappingModel
    {
        private readonly MockService _service;
        private readonly MockServiceFragment _fragment;

        private readonly ObservableCollection<AbstractMappingModel> _children;

        private readonly string _description;

        public FragmentMappingModel(MockService service, MockServiceFragment fragment)
        {
            _service = service;
            _fragment = fragment;
            _children = [];

            foreach (var mapping in _fragment.Mappings)
            {
                _children.Add(new EndpointMappingModel(_fragment, mapping));
            }

            _description = "";

            if (_fragment.IsOverride)
            {
                _description = "OVERRIDE";
                IsDescriptionVisible = true;
            }
        }

        public override IEnumerable<AbstractMappingModel> Children => [];

        public override string Name => _fragment.FragmentName;
        public override string Description => _description;
        public override bool HasErrors => _fragment.HasError;

        public override IEnumerable<EndpointMappingModel> GetEndpointList()
        {
            if (_fragment.HasError)
            {
                return [new EndpointMappingModel(_fragment, null)];
            }

            return _children.SelectMany(x => x.GetEndpointList()); ;
        }

        public MockServiceFragment Fragment => _fragment;

        public override IBrush DescriptionBrush => string.IsNullOrEmpty(_description) ? Brushes.Gray : CommonBrushes.Selected;
    }
}
