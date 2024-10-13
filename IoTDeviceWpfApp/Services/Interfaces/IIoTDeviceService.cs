using IoTDeviceWpfApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTDeviceWpfApp.Services
{
    public interface IIoTDeviceService
    {
        Task SendTelemetryAsync(DeviceModel device, string telemetryData);
        Task UpdateDeviceTwinAsync(DeviceModel device, string propertyName, object value);
        Task<string> ReceiveMessagesAsync(DeviceModel device);
    }
}
