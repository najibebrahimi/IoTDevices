using AzureIoTHubConsoleApp.IoTDeviceManager.Models;
using AzureIoTHubConsoleApp.IoTDeviceManager.Models.DataObjects;

namespace AzureIoTHubConsoleApp.IoTDeviceManager.Services;

public class MultiDeviceManager
{
    private readonly List<Models.IoTBaseDevice> _devices = new List<Models.IoTBaseDevice>();
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    
    public void AddDevice(IoTBaseDevice device)
    {
        _devices.Add(device);
        Console.WriteLine($"Device {device.DeviceID} added");
    }

    public void RemoveDevice(string deviceId)
    {
        throw new NotImplementedException("Not yet implemented!");
    }

    private IoTBaseDevice GetDevice(string deviceId)
    {
        foreach (var device in _devices)
        {
            if (deviceId.Equals(device.DeviceID))
            {
                return device;
            }
        }

        return null;
    }
    
    public async Task SendDataPayloadToDeviceAsync(string deviceId, DeviceData data, CancellationToken token)
    {
        var device = GetDevice(deviceId);
        if (device != null)
            await device.SendMessageToDeviceAsync(data, token);
    }

    public async Task StartReceivingMessagesForAllDevices()
    {
        var tasks = new List<Task>();

        foreach (var device in _devices)
            tasks.Add(device.ReceiveMessagesAsync(_cts.Token));

        await Task.WhenAll(tasks);
    }

    public async Task StartSendingHeartbeatMessagesForAllDevices()
    {
        var tasks = new List<Task>();

        foreach (var device in _devices)
            tasks.Add(device.CheckHeartbeatAsync(_cts.Token));

        await Task.WhenAll(tasks);
            
    }

    public void StopSendingHeartbeatMessages()
    { 
        _cts.Cancel();
    }

    public void StopReceivingMessages()
    {
        _cts.Cancel();
    }


}
