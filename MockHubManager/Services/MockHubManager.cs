using EmailService;
using IoTHubManager.Services;
using IoTMockHubManager.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;

namespace IoTMockHubManager.Services;

public class MockHubManager : IHubManager
{
    private EmailService.EmailService? _emailService;
    public string IoTHubConnectionString { get; set; }

    public MockHubManager(string iotHubConnectionString, string emailServiceConnectionString = null) 
    {        
        IoTHubConnectionString = iotHubConnectionString;
        if (!string.IsNullOrEmpty(emailServiceConnectionString))
            _emailService = new EmailService.EmailService(emailServiceConnectionString);
    }

    public async Task DeleteDeviceAsync(string deviceId)
    {
        return;
    }

    public async Task<List<Twin>> GetAllDevicesAsync()
    {
        // Simulate a delay like an actual async operation
        await Task.Delay(500);

        // Return a mock list of devices (Twins)
        var mockDevices = new List<Twin>
        {
            new MockTwin(
                deviceId: "42d3f02b-2a67-4220-ade4-f6b0c08abccc",
                DeviceStatus.Enabled,
                new Dictionary<string, object>
                {
                    { "On", true }
                }),
            new MockTwin(
                deviceId : "aa234f79-d786-4ad9-95dd-09d860b27b7e",
                DeviceStatus.Disabled,
                new Dictionary<string, object>
                {
                    { "On", false }
                }),
        };

        return mockDevices;
    }

    public Task SendC2DMessage(string deviceId, string message)
    {
        throw new NotImplementedException();
    }

    public async Task SendEmailOnDeviceDeletion(string deviceId, string senderEmail, string recipientEmail)
    {
        var subject = $"Device {deviceId} Deleted from IoT Hub";
        var plainTextContent = $"The device with ID {deviceId} has been successfully removed from the Azure IoT Hub.";
        var htmlContent = $"<strong>The device with ID {deviceId} has been successfully removed from the Azure IoT Hub.</strong>";

        if (_emailService != null)
            await _emailService.SendEmailAsync(senderEmail, recipientEmail, subject, plainTextContent, htmlContent);
        else
            Console.WriteLine($"No email service configured - mock sending the following text: {plainTextContent}");
    }

    public void SetupServiceClientAndRegistryManager()
    {
        return;
    }

    public void Dispose()
    {
        return;
    }
}
