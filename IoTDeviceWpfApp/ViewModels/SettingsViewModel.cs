using CommunityToolkit.Mvvm.Input;
using IoTDeviceManager.Services;
using IoTDeviceWpfApp.Core;
using IoTDeviceWpfApp.Models;
using IoTDeviceWpfApp.Services;
using System.Diagnostics;
using System.Windows;

namespace IoTDeviceWpfApp.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly IDeviceSettingsService _deviceSettingsService;
    private string _connectionString;

    public string ConnectionString
    {
        get => _connectionString;
        set
        {
            _connectionString = value;
            OnPropertyChanged(nameof(ConnectionString));
        }
    }

    private ISingleDeviceManager _deviceManager;

    public RelayCommand SaveCommand { get; }

    public SettingsViewModel(ISingleDeviceManager deviceManager, IDeviceSettingsService deviceSettingsService)
    {
        _deviceManager = deviceManager;
        _deviceSettingsService = deviceSettingsService;

        ConnectionString = Settings.Default.ConnectionString;

        SaveCommand = new RelayCommand(SaveSettings);
    }

    private void SaveSettings()
    {
        var settings = new DeviceSettings
        {
            ConnectionString = ConnectionString,
        };

        _deviceSettingsService.SaveSettings(settings);

        var result = MessageBox.Show(
            "Settings saved. Press OK to restart the application",
            "Restart Required",
            MessageBoxButton.OK);

        if (result == MessageBoxResult.OK)
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }
    }
}