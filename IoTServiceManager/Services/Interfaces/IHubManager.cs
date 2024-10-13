using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubManager.Services
{
    public interface IHubManager
    {
        Task SendC2DMessage(string deviceId, string message);

        Task DeleteDeviceAsync(string deviceId);

        Task<List<Twin>> GetAllDevicesAsync();

        Task SendEmailOnDeviceDeletion(string deviceId, string senderEmail, string recipientEmail);
        void SetupServiceClientAndRegistryManager();
        void Dispose();
    }
}
