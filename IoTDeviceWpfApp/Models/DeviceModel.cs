using AzureIoTHubConsoleApp.FanDevice.Models;
using AzureIoTHubConsoleApp.IoTDeviceManager.Models;
using AzureIoTHubConsoleApp.IoTDeviceManager.Models.DataObjects;
using IoTDeviceWpfApp.Services;
using Microsoft.Azure.Devices.Shared;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace IoTDeviceWpfApp.Models
{
    public class DeviceModel : INotifyPropertyChanged
    {
        private Fan _iotDevice;
        public string DeviceID => _iotDevice.DeviceID;        
        public string ConnectionString { get; set; }
        public string DeviceStatus { get; set; }

        public DateTime LastMessageReceivedAt => _iotDevice.LastMessageReceivedAt;
        public string LastMessageID => _iotDevice.LastMessageID;
        public string TelemetryData => _iotDevice.ToString();
        
        /*public DeviceModel(Fan iotDevice)
        {
           _iotDevice = iotDevice;
        }*/

        public DeviceModel() { }

        public DeviceModel(DeviceSettings config)
        {
            ConnectionString = config.ConnectionString;
            _iotDevice = new Fan(ConnectionString);
        }

        public async Task SendTelemetryAsync(DeviceData data, CancellationToken token)
        {
            await _iotDevice.SendMessageToDeviceAsync(data, token);
            OnPropertyChanged(nameof(LastMessageReceivedAt));
            OnPropertyChanged(nameof(LastMessageID));
        }

        public Twin GetDeviceTwinProperties(CancellationToken token)
        {
            var getTwinPropertiesTask = _iotDevice.GetDeviceTwinPropertiesAsync(token);
            return getTwinPropertiesTask.Result;
        }

        public async Task StartReceivingMessagesAsync(CancellationToken token)
        {
            await _iotDevice.ReceiveMessagesAsync(token);
            OnPropertyChanged(nameof(LastMessageReceivedAt));
            OnPropertyChanged(nameof(LastMessageID));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
