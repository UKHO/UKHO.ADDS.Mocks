using ADDSMock.Applications.Interactive.Controls;
using Avalonia.Media;
using ReactiveUI;

namespace ADDSMock.Applications.Interactive.Views.Mappings.Models
{
    internal abstract class AbstractMappingModel : ReactiveObject
    {
        private bool _isExpanded;
        private bool _isDescriptionVisible;

        protected AbstractMappingModel()
        {
            _isExpanded = false;
            _isDescriptionVisible = false;
        }

        public abstract IEnumerable<AbstractMappingModel> Children { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract bool HasErrors { get; }

        public abstract IEnumerable<EndpointMappingModel> GetEndpointList();

        public virtual IBrush DescriptionBrush => Brushes.Gray;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }

        public bool IsDescriptionVisible
        {
            get => _isDescriptionVisible;
            set => this.RaiseAndSetIfChanged(ref _isDescriptionVisible, value);
        }

        public IconState IconState
        {
            get
            {
                if (HasErrors)
                {
                    return IconState.Warning;
                }

                return IconState.Ok;
            }
        }
    }
}
