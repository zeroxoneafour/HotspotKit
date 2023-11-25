using HotspotKit.Services;
using Splat;

namespace HotspotKit.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IWiFiAdapterService _wifiAdapterService = Locator.Current.GetService<IWiFiAdapterService>()!;
    public MainWindowViewModel()
    {
        Manager = new ManagerViewModel(_wifiAdapterService);
    }

    public ManagerViewModel Manager { get; private set; }
}