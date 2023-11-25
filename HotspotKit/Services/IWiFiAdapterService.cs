using System.Collections.Generic;
using HotspotKit.Models;

namespace HotspotKit.Services;

public interface IWiFiAdapterService
{
    public bool IsAdministrator { get; }
    public IEnumerable<IAdapter> Adapters { get; }
    public bool IsRunning { get; }
    public int AdapterIndex { get; set; }
    public void StartHotspot(int sourceIndex, bool disableAutoConf);
    public void StopHotspot();
}