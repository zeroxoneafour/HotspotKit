using System.Collections.Generic;
using HotspotKit.Models;

namespace HotspotKit.Services;

public interface IWiFiAdapterService
{
    public IEnumerable<IAdapter> Adapters { get; }
    public bool IsRunning { get; }
    public void StartHotspot(int adapterIndex, bool disableAutoConf);
    public void StopHotspot();
}