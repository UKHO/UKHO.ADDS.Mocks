using Avalonia.Controls;
using ReactiveUI;

namespace ADDSMock.Applications.Interactive.Services
{
    public interface IViewService
    {
        object GetModel(Type modelType);

        T GetModel<T>() where T : ReactiveObject;

        T GetViewFor<T>(object model) where T : Control;

        Control GetViewFor(object model);

        T GetWindowFor<T>(object model) where T : Window;

        Window GetWindowFor(object model);
    }
}
