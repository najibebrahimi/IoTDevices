using AzureIoTHubConsoleApp.IoTDeviceManager.Models.DataObjects;
using AzureIoTHubConsoleApp.IoTDeviceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTDeviceManager.Services
{
    public interface IMultiDeviceManager
    {
        void AddDevice(IoTBaseDevice device);

        void RemoveDevice(string deviceId);

        Task SendDataPayloadToDeviceAsync(string deviceId, DeviceData data);

        Task StartReceivingMessagesForAllDevices();

        Task StartSendingHeartbeatMessagesForAllDevices();

        void StopSendingHeartbeatMessages();

        void StopReceivingMessages();
    }
}
