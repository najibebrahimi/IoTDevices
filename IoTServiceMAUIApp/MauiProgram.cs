using Microsoft.Extensions.Logging;
using IoTHubManager.Services;
using IoTMockHubManager.Services;
using IoTServiceMAUIApp.ViewModels;
using IoTServiceMAUIApp.Views;

namespace IoTServiceMAUIApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<DevicesViewModel>();
            builder.Services.AddTransient<AllDevicesPage>();
            
            var iotHubConnectionString = Preferences.Get("ConnectionString", String.Empty);
            var emailServiceConnectionString = Preferences.Get("EmailServiceConnectionString", String.Empty);

            //builder.Services.AddSingleton<IHubManager>(sp => new MockHubManager(iotHubConnectionString, emailServiceConnectionString));
            builder.Services.AddSingleton<IHubManager>(sp => new HubManager(iotHubConnectionString, emailServiceConnectionString));
            builder.Services.AddTransient<SettingsViewModel>(sp => new SettingsViewModel(sp));
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
