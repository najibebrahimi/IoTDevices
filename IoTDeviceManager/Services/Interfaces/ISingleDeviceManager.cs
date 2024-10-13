using AzureIoTHubConsoleApp.IoTDeviceManager.Models.DataObjects;
using IoTDeviceManager.Models;
using Microsoft.Azure.Devices.Shared;

namespace IoTDeviceManager.Services
{
    public interface ISingleDeviceManager
    {
        Task SendTelemetryAsync(string message, CancellationToken token);
        Task SendTelemetryAsync(DeviceData data, CancellationToken token);
        Task UpdateDeviceTwin(CustomTwinCollection collection, CancellationToken token);
        Task<Twin> GetDeviceTwinProperties(CancellationToken token);
        Task<ResponseResult> StartSendingHeartbeatMessages(CancellationToken token);

        string DeviceID { get; }
    }
}
