using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HotspotKit.ViewModels;
using HotspotKit.Views;
using Splat;
using Application = Avalonia.Application;
using SizeToContent = Avalonia.Controls.SizeToContent;

namespace HotspotKit;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Locator.Current.GetService<MainWindowViewModel>(),
                SizeToContent = SizeToContent.WidthAndHeight,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}