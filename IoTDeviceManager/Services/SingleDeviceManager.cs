using AzureIoTHubConsoleApp.IoTDeviceManager.Models;
using AzureIoTHubConsoleApp.IoTDeviceManager.Models.DataObjects;
using IoTDeviceManager.Models;
using Microsoft.Azure.Devices.Shared;


namespace IoTDeviceManager.Services
{
    public class SingleDeviceManager : ISingleDeviceManager
    {
        private IoTBaseDevice _device;
        public string DeviceID => _device.DeviceID;

        public SingleDeviceManager(IoTBaseDevice device) 
        {
            _device = device;
        }

        public async Task<Twin> GetDeviceTwinProperties(CancellationToken token)
        {
            return await _device.GetDeviceTwinPropertiesAsync(token);
        }

        public async Task SendTelemetryAsync(string message, CancellationToken token)
        {
            await _device.SendMessageToDeviceAsync(message, token);
        }

        public async Task SendTelemetryAsync(DeviceData data, CancellationToken token)
        {
            await _device.SendMessageToDeviceAsync(data, token);
        }

        public async Task UpdateDeviceTwin(CustomTwinCollection collection, CancellationToken token)
        {
            await _device.UpdateDeviceTwinPropertiesAsync(collection);
        }

        public async Task<ResponseResult> StartSendingHeartbeatMessages(CancellationToken token)
        {
            return await _device.CheckHeartbeatAsync(token);
        }
    }
}
