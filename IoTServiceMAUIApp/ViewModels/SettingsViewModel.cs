using System.Windows.Input;

namespace IoTServiceMAUIApp.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private string _emailAddress;
    private string _senderEmailAddress;
    private string _connectionString;
    private string _emailServiceConnectionString;
    
    // Add a service provider for dynamic service reloading
    private readonly IServiceProvider _serviceProvider;

    public SettingsViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        ConnectionString = Preferences.Get(nameof(ConnectionString), string.Empty);
        EmailServiceConnectionString = Preferences.Get(nameof(EmailServiceConnectionString), string.Empty);
        EmailAddress = Preferences.Get(nameof(EmailAddress), string.Empty);
        SenderEmailAddress = Preferences.Get(nameof(SenderEmailAddress), string.Empty);

        SaveCommand = new Command(SaveSettings);
    }

    public ICommand SaveCommand { get; }

    public string EmailAddress
    {
        get => _emailAddress;
        set
        {
            _emailAddress = value;
            OnPropertyChanged();
        }
    }

    public string SenderEmailAddress
    {
        get => _senderEmailAddress;
        set
        {
            _senderEmailAddress = value;
            OnPropertyChanged();
        }
    }

    public string ConnectionString
    {
        get => _connectionString;
        set
        {
            _connectionString = value;
            OnPropertyChanged();
        }
    }

    public string EmailServiceConnectionString
    {
        get => _emailServiceConnectionString;
        set
        {
            _emailServiceConnectionString = value;
            OnPropertyChanged();
        }
    }

    private void SaveSettings()
    {
        Preferences.Set(nameof(EmailAddress), EmailAddress);
        Preferences.Set(nameof(ConnectionString), ConnectionString);
        Preferences.Set(nameof(SenderEmailAddress), SenderEmailAddress);
        Preferences.Set(nameof(EmailServiceConnectionString), EmailServiceConnectionString);

        App.Current.MainPage.DisplayAlert("Settings", "Settings saved successfully!", "OK");

        // Notify to reload the HubManager service
        //ReloadHubManagerService();

        // Notify that the settings have been updated using MessagingCenter
        //MessagingCenter.Send(this, "SettingsUpdated");
    }

    /*private void ReloadHubManagerService()
    {
        // Reload or re-register the HubManager service with the new connection string
        var connectionString = Preferences.Get(nameof(ConnectionString), string.Empty);
        var emailServiceConnectionString = Preferences.Get(nameof(EmailServiceConnectionString), string.Empty);

        // Re-register the HubManager with the updated connection string
        var hubManager = new HubManager(connectionString, emailServiceConnectionString);
        _serviceProvider.GetRequiredService<IHubManager>(); // Reload service if needed
    }*/
}
