using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTDeviceManager.Exceptions
{
    public class DeviceDoesNotExistException : Exception
    {
        public DeviceDoesNotExistException()
        {
        }

        public DeviceDoesNotExistException(string message)
            : base(message)
        {
        }

        public DeviceDoesNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
