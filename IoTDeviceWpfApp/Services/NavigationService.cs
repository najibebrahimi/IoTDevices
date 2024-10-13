using IoTDeviceWpfApp.Core;

namespace IoTDeviceWpfApp.Services;

public class NavigationService : ObservableObject, INavigationService
{
    private readonly Func<Type, ViewModelBase> _viewModelFactory;
    private ViewModelBase _currentView;

    public ViewModelBase CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
    { 
        _viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<TViewModel>(params object[] parameters) where TViewModel : ViewModelBase
    {
        var viewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel), parameters);
        CurrentView = viewModel;
    }
}
