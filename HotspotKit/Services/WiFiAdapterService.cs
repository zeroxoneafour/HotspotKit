using System;
using System.Collections.Generic;
using Windows.Devices.WiFi;
using System.Linq;
using Windows.Devices.Enumeration;
using HotspotKit.Models;
using Microsoft.VisualBasic.Logging;
using Splat;

namespace HotspotKit.Services;

public class WiFiAdapterService : IWiFiAdapterService, IEnableLogger
{
    private List<Adapter>? _adapters;
    private NetShService _netShService = new();
    private IEnumerable<IAdapter> GetAdapters()
    {
        _adapters = new List<Adapter>();
        var wifiAdapters = DeviceInformation
            .FindAllAsync(WiFiAdapter.GetDeviceSelector())
            .GetAwaiter()
            .GetResult();
        foreach (var a in wifiAdapters)
        {
            try
            {
                var networkAdapter = WiFiAdapter.FromIdAsync(a.Id).GetAwaiter().GetResult().NetworkAdapter;
                var netShInfo = _netShService.GetInfo(networkAdapter.NetworkAdapterId.ToString());
                var adapter = new Adapter(networkAdapter, netShInfo);
                _adapters.Add(adapter);
            }
            catch (Exception e)
            {
                this.Log().Error(e);
            }
        }
        return _adapters;
    }
    
    public IEnumerable<IAdapter> Adapters => _adapters ?? GetAdapters();

    public bool IsRunning => _runningAdapter is not null;
    private Adapter? _runningAdapter = null;
    public void StartHotspot(int adapterIndex, bool disableAutoConf)
    {
        _runningAdapter = _adapters[adapterIndex];
        this.Log().Debug("Hotspot starting with adapter " + _runningAdapter.Name);
        this.Log().Debug(disableAutoConf);
        _netShService.DisconnectInterface(_runningAdapter.Name);
        if (disableAutoConf)
        {
            _netShService.DisableAutoConfig(_runningAdapter.Name);
        }
        _runningAdapter.StartTethering();
        this.Log().Debug("Hotspot started");
    }

    public void StopHotspot()
    {
        this.Log().Debug("Hotspot stopping");
        _runningAdapter.StopTethering();
        _netShService.ConnectInterface(_runningAdapter.Name, _runningAdapter.ProfileName);
        _netShService.EnableAutoConfig(_runningAdapter.Name);
        this.Log().Debug("Hotspot stopped");
        _runningAdapter = null;
    }
}