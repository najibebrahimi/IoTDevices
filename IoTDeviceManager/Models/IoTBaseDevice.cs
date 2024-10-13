using AzureIoTHubConsoleApp.IoTDeviceManager.Models.DataObjects;
using IoTDeviceManager.Exceptions;
using IoTDeviceManager.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Text;
using System.Text.RegularExpressions;

namespace AzureIoTHubConsoleApp.IoTDeviceManager.Models
{
    public abstract class IoTBaseDevice
    {
        public string ConnectionString { get; set; }
        private DeviceClient? _deviceClient;

        public string DeviceID { get; private set; }
        public DateTime LastMessageReceivedAt { get; set; }
        public string LastMessageID { get; set; }

        public IoTBaseDevice(string? connectionString) 
        {
            ConnectionString = connectionString;

            if (!string.IsNullOrEmpty(connectionString))
                SetupDeviceIdAndClient();
            else
            {
                DeviceID = "No device";
                Console.WriteLine("Warning, no connection string set");
            }

            LastMessageReceivedAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            LastMessageID = "N/A";
        }

        public void SetupDeviceIdAndClient()
        {
            var match = Regex.Match(ConnectionString, @"DeviceId=([^;]+)");
            if (match.Success)
                DeviceID = match.Groups[1].Value;
            else
                throw new ArgumentException("Connection string must contain `DeviceId`");
            _deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString);
        }

        public async Task SendMessageToDeviceAsync(string message, CancellationToken token)
        {
            try
            {
                var cloudToDeviceMessage = new Microsoft.Azure.Devices.Client.Message(System.Text.Encoding.UTF8.GetBytes(message))
                {
                    ContentEncoding = "utf-8",
                    ContentType = "application/json",
                };

                await _deviceClient.SendEventAsync(cloudToDeviceMessage, token);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { }
        }

        public async Task ReceiveMessagesAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Message receivedMessage = await _deviceClient.ReceiveAsync();
                if (receivedMessage != null)
                {
                    string messageData = Encoding.UTF8.GetString(receivedMessage.GetBytes());
                    Console.WriteLine($"Received message from {DeviceID}: {messageData}");

                    ProcessDeviceData(messageData);

                    Console.WriteLine($"Device Info: {this.ToString()}");

                    await _deviceClient.CompleteAsync(receivedMessage);
                }

                await Task.Delay(1000);
            }
        }

        public async Task<ResponseResult> CheckHeartbeatAsync(CancellationToken token)
        {
            //while (!token.IsCancellationRequested)
            //{
            try
            {
                var twin = await _deviceClient.GetTwinAsync();
                if (twin == null)
                    return new ResponseResult { Success = false, Error = true, Message = $"Device {DeviceID} does not exist!" };
                //throw new DeviceDoesNotExistException($"Device {DeviceID} does not exist!");

                return new ResponseResult { Success = true, Error = false, Message = $"Device {DeviceID} online" };
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("404001") || ex.Message.Contains("status-code: 401"))
                    return new ResponseResult { Success = false, Error = true, Message = $"Device {DeviceID} does not exist!" };

                return new ResponseResult { Success = false, Error = true, Message = ex.Message };
                //throw new DeviceDoesNotExistException($"Device {DeviceID} does not exist!");
            }

            //int delayMs = delaySeconds * 1000;
            //await Task.Delay(delayMs);
        //}
        }

        public abstract void ProcessDeviceData(string message);

        public abstract Task SendMessageToDeviceAsync(DeviceData data, CancellationToken token);

        public override string ToString()
        {
            return $"\nDevice ID: {DeviceID}\nLastMessageReceivedAt: {LastMessageReceivedAt}\nLastMessageID:{LastMessageID}\n";
        }

        public async Task UpdateDeviceTwinPropertiesAsync(CustomTwinCollection collection)
        {
            var twinCollection = collection.ToTwinCollection();
            await _deviceClient.UpdateReportedPropertiesAsync(twinCollection);
            Console.WriteLine("Device twin reported properties updated");
        }

        public async Task<Twin> GetDeviceTwinPropertiesAsync(CancellationToken token)
        {
            return await _deviceClient.GetTwinAsync(token);
        }
    }
}
