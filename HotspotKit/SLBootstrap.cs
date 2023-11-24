using HotspotKit.Services;
using HotspotKit.ViewModels;
using Splat;

namespace HotspotKit;

public class SLBootstrap
{
    public SLBootstrap()
    {
        var logger = new DebugLogger()
        {
            Level = LogLevel.Debug
        };
        Locator.CurrentMutable.RegisterConstant<ILogger>(logger);
        Locator.CurrentMutable.Register(() => new MainWindowViewModel());
        Locator.CurrentMutable.RegisterLazySingleton<IWiFiAdapterService>(() => new WiFiAdapterService());
    }
}