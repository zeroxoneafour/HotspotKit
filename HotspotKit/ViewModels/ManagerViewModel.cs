using System.Collections.ObjectModel;
using System.Windows.Input;
using HotspotKit.Models;
using HotspotKit.Services;
using ReactiveUI;

namespace HotspotKit.ViewModels;

public class ManagerViewModel : ViewModelBase
{
    private readonly IWiFiAdapterService _wifiAdapterService;

    public ManagerViewModel(IWiFiAdapterService wifiAdapterService)
    {
        _wifiAdapterService = wifiAdapterService;
        StartStopButtonCommand = ReactiveCommand.Create(this.StartStopButton);
    }

    public ObservableCollection<IAdapter> Adapters
        => new(_wifiAdapterService.Adapters);

    private void StartStopButton()
    {
        if (_wifiAdapterService.IsRunning)
        {
            IsStartStopEnabled = false;
            _wifiAdapterService.StopHotspot();
            StartStopButtonText = "Start";
            IsStartStopEnabled = true;
        }
        else
        {
            IsStartStopEnabled = false;
            _wifiAdapterService.StartHotspot(SourceIndex, DisableAutoConf);
            StartStopButtonText = "Stop";
            IsStartStopEnabled = true;
        }
    }

    public ICommand StartStopButtonCommand { get; private set; }

    private bool _isStartStopEnabled = true;

    public bool IsStartStopEnabled
    {
        get => _isStartStopEnabled;
        set => this.RaiseAndSetIfChanged(ref _isStartStopEnabled, value);
    }

    private string _startStopButtonText = "Start";

    public string StartStopButtonText
    {
        get => _startStopButtonText;
        set => this.RaiseAndSetIfChanged(ref _startStopButtonText, value);
    }

    private int _sourceIndex = 0;

    public int SourceIndex
    {
        get => _sourceIndex;
        set => this.RaiseAndSetIfChanged(ref _sourceIndex, value);
    }

    public int AdapterIndex
    {
        get => _wifiAdapterService.AdapterIndex;
        set
        {
            int a = 0;
            this.RaiseAndSetIfChanged(ref a, value);
            _wifiAdapterService.AdapterIndex = a;
        }
    }

    private bool _disableAutoConf = false;
    public bool DisableAutoConf
    {
        get => _disableAutoConf;
        set => this.RaiseAndSetIfChanged(ref _disableAutoConf, value);
    }

    public bool IsAdministrator => _wifiAdapterService.IsAdministrator;
}