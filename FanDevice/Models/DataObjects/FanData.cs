using AzureIoTHubConsoleApp.IoTDeviceManager.Models.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureIoTHubConsoleApp.FanDevice.Models.DataObjects
{
    public class FanData : DeviceData
    {
        public bool On { get; set; }
    }
}
