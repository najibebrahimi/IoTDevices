using IoTDeviceWpfApp.Models;

namespace IoTDeviceWpfApp.Services
{
    public interface IDeviceSettingsService
    {
        DeviceSettings LoadSettings();
        void SaveSettings(DeviceSettings settings);
    }
}
