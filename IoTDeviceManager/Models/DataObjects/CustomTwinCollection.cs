using Microsoft.Azure.Devices.Shared;

namespace AzureIoTHubConsoleApp.IoTDeviceManager.Models.DataObjects;

public class CustomTwinCollection
{
    private TwinCollection _twinCollection;

    public CustomTwinCollection()
    {
        _twinCollection = new TwinCollection();
    }

    public void Add(string key, object value)
    {
        _twinCollection[key] = value;
    }

    public object this[string key]
    {
        get => _twinCollection[key];
        set => _twinCollection[key] = value;
    }

    public TwinCollection ToTwinCollection()
    {
        return _twinCollection;
    }
}
