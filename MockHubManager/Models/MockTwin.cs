using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;

namespace IoTMockHubManager.Models;

public class MockTwin : Twin
{
    public new DeviceStatus Status { get; set; }

    public MockTwin(string deviceId, DeviceStatus status, Dictionary<string, object> reportedProperties)
    {
        DeviceId = deviceId;
        Status = status;
        
        foreach( var kvp in reportedProperties)
            Properties.Reported[kvp.Key] = kvp.Value;
    }
}
