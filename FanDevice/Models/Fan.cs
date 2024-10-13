using AzureIoTHubConsoleApp.FanDevice.Models.DataObjects;
using AzureIoTHubConsoleApp.IoTDeviceManager.Models;
using AzureIoTHubConsoleApp.IoTDeviceManager.Models.DataObjects;
using AzureIoTHubConsoleApp.FanDevice.Models.DataObjects;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace AzureIoTHubConsoleApp.FanDevice.Models
{
    public class Fan : IoTBaseDevice
    {
        public bool On { get; set; }
        
        public Fan(string connectionString) : base(connectionString)
        {
            On = false;
        }

        public override void ProcessDeviceData(string message)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<FanData>(message);

                if (data != null)
                {
                    LastMessageReceivedAt = DateTime.UtcNow;
                    LastMessageID = data.MessageID;
                    On = data.On;
                    Console.WriteLine($"Updated On state: {On}");

                    _ = UpdateDeviceTwinPropertiesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        public override string ToString()
        {
            var deviceInfo = base.ToString();

            return deviceInfo += $"\nOn: {On}\n";
        }

        public async Task SendDataPayloadToDeviceAsync(FanData data, CancellationToken token)
        {
            string message = JsonConvert.SerializeObject(data);
            Console.WriteLine($"Sending serialized message: {message}");
            await SendMessageToDeviceAsync(message, token);

            On = data.On;
            
            await UpdateDeviceTwinPropertiesAsync();
        }

        public override async Task SendMessageToDeviceAsync(DeviceData data, CancellationToken token)
        {
            await this.SendDataPayloadToDeviceAsync((FanData)data, token);
        }

        public async Task UpdateDeviceTwinPropertiesAsync()
        {
            var reportedProperties = new CustomTwinCollection
            {
                ["On"] = On
            };

            await base.UpdateDeviceTwinPropertiesAsync(reportedProperties);
        }
    }
}
