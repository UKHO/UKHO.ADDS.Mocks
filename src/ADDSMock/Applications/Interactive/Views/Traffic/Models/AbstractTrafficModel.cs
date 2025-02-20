using ADDSMock.Applications.Interactive.Views.Mappings.Models;
using ReactiveUI;

namespace ADDSMock.Applications.Interactive.Views.Traffic.Models
{
    internal abstract class AbstractTrafficModel : ReactiveObject
    {
        private bool _isMethodsVisible;
        private bool _isDescriptionVisible;
        private bool _isExpanded;

        protected AbstractTrafficModel()
        {
            _isExpanded = false;
            _isDescriptionVisible = false;
            _isMethodsVisible = false;
        }

        public abstract IEnumerable<AbstractTrafficModel> Children { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract IEnumerable<MethodModel> Methods { get; }

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

        public bool IsMethodsVisible
        {
            get => _isMethodsVisible;
            set => this.RaiseAndSetIfChanged(ref _isMethodsVisible, value);
        }
    }
}
