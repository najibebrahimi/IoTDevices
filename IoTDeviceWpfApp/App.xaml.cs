using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using IoTDeviceWpfApp.Core;
using IoTDeviceWpfApp.Services;
using IoTDeviceWpfApp.ViewModels;
using IoTDeviceWpfApp.Views;
using IoTDeviceManager.Services;
using AzureIoTHubConsoleApp.FanDevice.Models;

namespace IoTDeviceWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });
            
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<SettingsViewModel>();

            services.AddSingleton<INavigationService, Services.NavigationService>();
            services.AddSingleton<IDeviceSettingsService, DeviceSettingsService>();
            
            var connectionString = Settings.Default.ConnectionString;
            
            services.AddSingleton<ISingleDeviceManager>(new SingleDeviceManager(new Fan(connectionString)));

            services.AddSingleton<Func<Type, ViewModelBase>> (serviceProvider => viewModelType => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType));

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

    }

}
