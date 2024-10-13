using CommunityToolkit.Mvvm.Input;

using IoTDeviceManager.Services;
using IoTDeviceWpfApp.Core;
using IoTDeviceWpfApp.Models;
using IoTDeviceWpfApp.Services;

namespace IoTDeviceWpfApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IDeviceSettingsService _deviceSettingsService;
    private ISingleDeviceManager _deviceManager;
    private DeviceModel _device { get; set; }
    private INavigationService _navigation;
    private string _message;

    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            OnPropertyChanged(nameof(Message));
        }
    }
    
    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand NavigateToHomeViewCommand { get; }
    public RelayCommand NavigateToSettingsViewCommand { get; }
   
    public MainViewModel(INavigationService navService, IDeviceSettingsService deviceSettingsService, ISingleDeviceManager deviceManager)
    {
        Navigation = navService;
        _deviceSettingsService = deviceSettingsService;
        _deviceManager = deviceManager;

        NavigateToHomeViewCommand = new RelayCommand(NavigateToHomeView);
        NavigateToSettingsViewCommand = new RelayCommand(NavigateToSettingsView);

        Message = "Welcome to the IoT Device Manager!\n";
        
        var settings = _deviceSettingsService.LoadSettings();
        if (string.IsNullOrEmpty(settings.ConnectionString))
            Message += "No IoT Hub connection string set! Head over to Settings before proceeding.";
        else
            Message += "To view your device, visit 'Home', or head over to 'Settings' to update your information";
    }

    private void NavigateToHomeView()
    {
        Message = string.Empty;
        Navigation.NavigateTo<HomeViewModel>(_deviceManager);
    }

    private void NavigateToSettingsView()
    {
        Message = string.Empty;
        Navigation.NavigateTo<SettingsViewModel>(_deviceManager, _deviceSettingsService);
    }
}