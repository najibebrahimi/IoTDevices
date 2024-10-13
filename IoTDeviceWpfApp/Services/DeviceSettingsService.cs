using IoTDeviceWpfApp.Models;
using Microsoft.Azure.Amqp.Framing;

namespace IoTDeviceWpfApp.Services;

public class DeviceSettingsService : IDeviceSettingsService
{

    public DeviceSettings LoadSettings()
    {
        var connectionString = Settings.Default.ConnectionString;
        return new DeviceSettings { ConnectionString = connectionString };
    }

    public void SaveSettings(DeviceSettings settings)
    {
        Settings.Default.ConnectionString = settings.ConnectionString;
        Settings.Default.Save();
    }
}
