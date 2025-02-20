using Avalonia.Controls;
using ReactiveUI;

namespace ADDSMock.Applications.Interactive.Services.Implementation
{
    internal class ViewService : IViewService
    {
        private readonly IServiceProvider _provider;

        public ViewService(IServiceProvider provider) => _provider = provider;

        public object GetModel(Type modelType) => _provider.GetRequiredService(modelType);

        public T GetModel<T>() where T : ReactiveObject => _provider.GetRequiredService<T>();

        public T GetViewFor<T>(object model) where T : Control
        {
            var modelTypeName = model.GetType().Name;

            string viewTypeName;

            if (modelTypeName.EndsWith("ViewModel"))
            {
                viewTypeName = modelTypeName.Replace("Model", "");
            }
            else if (modelTypeName.EndsWith("DialogModel"))
            {
                viewTypeName = modelTypeName.Replace("Model", "");
            }
            else
            {
                viewTypeName = modelTypeName.EndsWith("Model")
                    ? modelTypeName.Replace("Model", "View")
                    : $"{modelTypeName}View";
            }

            var viewType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.Name == viewTypeName);

            if (viewType == null)
            {
                throw new ArgumentException($"Cannot find view for model type {model.GetType()}");
            }

            var view = (T)_provider.GetService(viewType)!;
            view.DataContext = model;

            return view;
        }

        public Control GetViewFor(object model) => GetViewFor<Control>(model);

        public T GetWindowFor<T>(object model) where T : Window
        {
            var modelTypeName = model.GetType().Name;

            string viewTypeName;

            if (modelTypeName.EndsWith("ViewModel"))
            {
                viewTypeName = modelTypeName.Replace("ViewModel", "");
            }
            else if (modelTypeName.EndsWith("Model"))
            {
                viewTypeName = modelTypeName.Replace("Model", "");
            }
            else
            {
                viewTypeName = modelTypeName.EndsWith("Model")
                    ? modelTypeName.Replace("Model", "Window")
                    : $"{modelTypeName}Window";
            }

            var viewType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.Name == viewTypeName);

            if (viewType == null)
            {
                throw new ArgumentException($"Cannot find window for model type {model.GetType()}");
            }

            var view = (T)_provider.GetService(viewType)!;
            view.DataContext = model;

            return view;
        }

        public Window GetWindowFor(object model) => GetWindowFor<Window>(model);
    }
}
