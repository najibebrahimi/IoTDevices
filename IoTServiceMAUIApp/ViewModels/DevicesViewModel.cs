using Microsoft.Azure.Devices.Shared;
using System.Collections.ObjectModel;
using System.Windows.Input;

using IoTHubManager.Services;
using EmailService.Exceptions;

namespace IoTServiceMAUIApp.ViewModels;

public class DevicesViewModel : ViewModelBase
{
    public ObservableCollection<Twin> AllDevices { get; } = new ObservableCollection<Twin>();
    private IHubManager _hubManager;
    public ICommand DeleteDeviceCommand { get; }

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public DevicesViewModel(IHubManager hubManager)
    {
        _hubManager = hubManager;

        if (string.IsNullOrEmpty(Preferences.Get("ConnectionString", string.Empty)))
            ErrorMessage = "No IoT Hub connection string set!";
        else
            LoadDevicesAsync();

        DeleteDeviceCommand = new Command<Twin>(async (device) => await DeleteDeviceAsync(device));
    }

    private async Task DeleteDeviceAsync(Twin device)
    {
        if (device != null)
        {
            ErrorMessage = string.Empty;

            try
            {
                string devId = device.DeviceId;
                await _hubManager.DeleteDeviceAsync(device.DeviceId);
                AllDevices.Remove(device);
                var senderEmailAddress = Preferences.Get("SenderEmailAddress", string.Empty);
                var recipientEmailAddress = Preferences.Get("EmailAddress", string.Empty);

                await _hubManager.SendEmailOnDeviceDeletion(devId, senderEmailAddress, recipientEmailAddress);
            }
            catch (EmailServiceException ex)
            {
                ErrorMessage = $"Failed to send email notification! - {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to remove device! - {ex.Message}";
            }
        }
    }

    private async void LoadDevicesAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            var devices = await _hubManager.GetAllDevicesAsync();

            AllDevices.Clear();

            foreach (var device in devices)
                AllDevices.Add(device);
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Error loading devices: {ex.Message}");
            ErrorMessage = $"Failed to load devices. Please try again. - {ex.Message}";
        }
    }

    private void ReloadHubManager()
    {
        var connectionString = Preferences.Get("ConnectionString", string.Empty);
        var emailServiceConnectionString = Preferences.Get("EmailServiceConnectionString", string.Empty);
        _hubManager = new HubManager(connectionString, emailServiceConnectionString);
    }

    public void OnAppearing()
    {
        ErrorMessage = string.Empty;
        if (!string.IsNullOrEmpty(Preferences.Get("ConnectionString", string.Empty)))
        {
            ReloadHubManager();
            LoadDevicesAsync();
        }
        else
            ErrorMessage = "No IoT Hub connection string set!";
    }
}
