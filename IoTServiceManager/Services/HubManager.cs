using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Text;
using EmailService;

namespace IoTHubManager.Services;

public class HubManager : IHubManager
{
    private ServiceClient _serviceClient;
    private RegistryManager _registryManager;
    private EmailService.EmailService? _emailService;
    public string IoTHubConnectionString { get; set; }

    public HubManager(string iotHubConnectionString, string emailServiceConnectionString = null)
    {
        IoTHubConnectionString = iotHubConnectionString;
        if (!string.IsNullOrEmpty(IoTHubConnectionString))
            SetupServiceClientAndRegistryManager();
        if (!string.IsNullOrEmpty(emailServiceConnectionString))
            _emailService = new EmailService.EmailService(emailServiceConnectionString);
    }

    public void SetupServiceClientAndRegistryManager()
    {
        _serviceClient = ServiceClient.CreateFromConnectionString(IoTHubConnectionString);
        _registryManager = RegistryManager.CreateFromConnectionString(IoTHubConnectionString);
    }

    public async Task SendC2DMessage(string deviceId, string message)
    {
        var serializedMessage = JsonConvert.SerializeObject(message);
        var iotMessage = new Message(Encoding.UTF8.GetBytes(serializedMessage));

        await _serviceClient.SendAsync(deviceId, iotMessage);
    }

    public async Task DeleteDeviceAsync(string deviceId)
    {
        await _registryManager.RemoveDeviceAsync(deviceId);
    }

    public async Task<List<Twin>> GetAllDevicesAsync()
    {
        var twins = new List<Twin>();
        var query = _registryManager.CreateQuery("SELECT * FROM devices");
        while (query.HasMoreResults)
        {
            var twinList = await query.GetNextAsTwinAsync();
            twins.AddRange(twinList);
        }

        return twins;
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

    public void Dispose()
    {
        _serviceClient?.Dispose();
        _registryManager?.Dispose();
    }
}
