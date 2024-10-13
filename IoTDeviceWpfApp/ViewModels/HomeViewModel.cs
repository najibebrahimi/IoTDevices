using CommunityToolkit.Mvvm.Input;
using IoTDeviceWpfApp.Core;
using IoTDeviceManager.Services;
using AzureIoTHubConsoleApp.FanDevice.Models.DataObjects;
using AzureIoTHubConsoleApp.IoTDeviceManager.Models.DataObjects;
using Microsoft.Azure.Devices.Shared;
using System.Windows.Threading;
using System.Threading;
using IoTDeviceManager.Models;

namespace IoTDeviceWpfApp.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private ISingleDeviceManager _deviceManager;
    private bool _isFanOn;
    private DispatcherTimer _timer;
    private double _angle;
    private string _deviceId = "N/A";
    private CancellationTokenSource _cancellationTokenSource;
    private DateTime? _deviceLastSeen;
    private string _deviceStatus = "N/A";
    private int _heartbeatUpdateIntervalSeconds = 10;
    private DateTime? _lastTelemetrySent;

    public String LastTelemetrySentText
    {
        get
        {
            return _lastTelemetrySent.HasValue ? _lastTelemetrySent.Value.ToString() : "N/A";
        }
    }

    public String DeviceLastSeenText
    {
        get
        {
            return _deviceLastSeen.HasValue ? _deviceLastSeen.Value.ToString() : "N/A";
        }
    }

    public DateTime? LastTelemetrySent
    {
        get => _lastTelemetrySent;
        set
        {
            _lastTelemetrySent = value;
            OnPropertyChanged(nameof(LastTelemetrySent));
            OnPropertyChanged(nameof(LastTelemetrySentText));
        }
    }

    public int HeartbeatUpdateIntervalSeconds => _heartbeatUpdateIntervalSeconds;

    public string DeviceStatus
    {
        get => _deviceStatus;
        set
        {
            _deviceStatus = value;
            OnPropertyChanged(nameof(DeviceStatus));
        }
    }

    public DateTime? DeviceLastSeen
    {
        get => _deviceLastSeen;
        set
        {
            _deviceLastSeen = value;
            OnPropertyChanged(nameof(DeviceLastSeen));
            OnPropertyChanged(nameof(DeviceLastSeenText));
        }
    }

    public bool IsFanOn
    {
        get => _isFanOn;
        set
        {
            _isFanOn = value;
            OnPropertyChanged(nameof(IsFanOn));
        }
    }

    public string DeviceId
    {
        get => _deviceId;
        set
        {
            _deviceId = value;
            OnPropertyChanged(nameof(DeviceId));
        }
    }

    public string DeviceID => _deviceManager.DeviceID;

    public RelayCommand SendTelemetryCommand { get; set; }
    public RelayCommand ToggleFanOnCommand { get; set; }
    public RelayCommand ToggleFanOffCommand { get; set; }
    public RelayCommand GetDeviceTwinCommand { get; set; }
    public RelayCommand StartReceivingMessagesCommand { get; }
    public RelayCommand StopReceivingMessagesCommand { get; }

    public Twin TempTwin { get; set; }

    public HomeViewModel(ISingleDeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
        DeviceId = deviceManager.DeviceID;

        // Initialize the timer
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(50); // Adjust rotation speed
        _timer.Tick += RotateFanBlades;

        SendTelemetryCommand = new RelayCommand(SendTelemetry);
        ToggleFanOnCommand = new RelayCommand(ToggleFanOn);
        ToggleFanOffCommand = new RelayCommand(ToggleFanOff);
        GetDeviceTwinCommand = new RelayCommand(GetDeviceTwin);
        StartReceivingMessagesCommand = new RelayCommand(StartHeartbeat);
        StopReceivingMessagesCommand = new RelayCommand(StopHeartbeat);

        LoadDeviceState();

        if (StartReceivingMessagesCommand.CanExecute(null))
            StartReceivingMessagesCommand.Execute(null);
    }

    private async void SendTelemetry()
    {
        var cts = new CancellationTokenSource();

        try
        {
            var data = new FanData
            {
                MessageID = Guid.NewGuid().ToString(),
                On = true
            };
            await _deviceManager.SendTelemetryAsync(data, cts.Token);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { }
        finally
        {
            cts.Dispose();
        }
    }

    private async void ToggleFanOn()
    {
        var cts = new CancellationTokenSource();

        var data = new FanData
        {
            MessageID = Guid.NewGuid().ToString(),
            On = true
        };

        try
        {
            await _deviceManager.SendTelemetryAsync(data, cts.Token);

            var reportedProperties = new CustomTwinCollection
            {
                ["On"] = true
            };

            await _deviceManager.UpdateDeviceTwin(reportedProperties, cts.Token);

            _timer.Start();
            IsFanOn = true;
            LastTelemetrySent = DateTime.Now;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { }
        finally
        {
            cts.Dispose();
        }
    }

    private async void ToggleFanOff()
    {
        var cts = new CancellationTokenSource();

        var data = new FanData
        {
            MessageID = Guid.NewGuid().ToString(),
            On = false
        };

        try
        {
            await _deviceManager.SendTelemetryAsync(data, cts.Token);
            var reportedProperties = new CustomTwinCollection
            {
                ["On"] = false
            };

            await _deviceManager.UpdateDeviceTwin(reportedProperties, cts.Token);

            _timer.Stop();
            IsFanOn = false;
            LastTelemetrySent = DateTime.Now;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) 
        {
            
        }
        finally
        {
            cts.Dispose();
        }
    }

    private async void LoadDeviceState()
    {
        var cts = new CancellationTokenSource();

        try
        {
            TempTwin = await _deviceManager.GetDeviceTwinProperties(cts.Token);

            if (TempTwin.Properties.Reported.Contains("On"))
            {
                bool fanOn = (bool)TempTwin.Properties.Reported["On"];
                if (fanOn)
                    _timer.Start();
                IsFanOn = fanOn;
                DeviceLastSeen = DateTime.Now;
                DeviceStatus = "Online";
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) 
        {
            if (ex.Message.Contains("404001") || ex.Message.Contains("status-code: 401"))
            {
                DeviceStatus = "Device does not exist!";
            }
            else
                DeviceStatus = "Offline";
        }
        finally
        {
            cts.Dispose();
        }
    }

    private async void GetDeviceTwin()
    {
        var cts = new CancellationTokenSource();

        try
        {
            TempTwin = await _deviceManager.GetDeviceTwinProperties(cts.Token);
            OnPropertyChanged(nameof(TempTwin));
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { }
        finally
        {
            cts.Dispose();
        }
    }

    public async void StartHeartbeat()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                TempTwin = await _deviceManager.GetDeviceTwinProperties(_cancellationTokenSource.Token);
                OnPropertyChanged(nameof(TempTwin));
                DeviceLastSeen = DateTime.Now;
                DeviceStatus = "Online";
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) 
            {
                if (ex.Message.Contains("404001") || ex.Message.Contains("status-code: 401"))
                    DeviceStatus = "Device does not exist!";
                else
                    DeviceStatus = "Offline";
            }

            await Task.Delay(_heartbeatUpdateIntervalSeconds * 1000);
        }
    }

    public void StopHeartbeat()
    {
        _cancellationTokenSource.Cancel();
    }

    private void RotateFanBlades(object sender, EventArgs e)
    {
        _angle += 10;
        if (_angle >= 360) _angle = 0;

        OnPropertyChanged(nameof(Blade1Angle));
        OnPropertyChanged(nameof(Blade2Angle));
        OnPropertyChanged(nameof(Blade3Angle));
    }

    public double Blade1Angle => _angle;
    public double Blade2Angle => _angle + 120;
    public double Blade3Angle => _angle + 240;
}
