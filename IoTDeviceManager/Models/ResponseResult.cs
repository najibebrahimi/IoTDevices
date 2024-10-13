using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTDeviceManager.Models
{
    public class ResponseResult
    {
        public bool Success { get; set; }
        public bool Error { get; set; }
        public string? Message { get; set; }
    }
}
