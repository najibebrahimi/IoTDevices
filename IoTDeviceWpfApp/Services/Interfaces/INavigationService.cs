using IoTDeviceWpfApp.Core;

namespace IoTDeviceWpfApp.Services
{
    public interface INavigationService
    {
        ViewModelBase CurrentView { get; }
        void NavigateTo<T>(params object[] parameters) where T : ViewModelBase;
    }
}
