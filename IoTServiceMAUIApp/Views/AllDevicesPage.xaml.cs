using IoTServiceMAUIApp.ViewModels;

namespace IoTServiceMAUIApp.Views;

public partial class AllDevicesPage : ContentPage
{
    private DevicesViewModel _viewModel;

    public AllDevicesPage(DevicesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}
