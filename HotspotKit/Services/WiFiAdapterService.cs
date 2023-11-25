using System;
using System.Collections.Generic;
using Windows.Devices.WiFi;
using System.Security.Principal;
using Windows.Devices.Enumeration;
using HotspotKit.Models;
using Splat;

namespace HotspotKit.Services;

public class WiFiAdapterService : IWiFiAdapterService, IEnableLogger
{
    public bool IsAdministrator { get; private set; }
    private List<Adapter>? _adapters;
    private NetShService _netShService = new();
    private RegistryService _registryService = new();

    public WiFiAdapterService()
    {
        IsAdministrator = (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
            .IsInRole(WindowsBuiltInRole.Administrator);
        GetAdapters();
        _adapterIndex = GetAdapterIndex();
    }
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

    public bool IsRunning => _sourceAdapter is not null;
    private Adapter? _sourceAdapter = null;
    private Adapter? _runningAdapter = null;
    public void StartHotspot(int sourceAdapter, bool disableAutoConf)
    {
        _sourceAdapter = _adapters[sourceAdapter];
        if (_adapterIndex < 0)
        {
            throw new Exception("Cannot find any running adapter!");
        }
        _runningAdapter = _adapters[_adapterIndex];
        this.Log().Debug("Hotspot starting with adapter " + _sourceAdapter.Name);
        if (disableAutoConf && _runningAdapter is not null)
        {
            _netShService.DisconnectInterface(_runningAdapter.Name);
            _netShService.DisableAutoConfig(_runningAdapter.Name);
        }
        _sourceAdapter.StartTethering();
        this.Log().Debug("Hotspot started");
    }

    public void StopHotspot()
    {
        this.Log().Debug("Hotspot stopping");
        _sourceAdapter.StopTethering();
        if (_runningAdapter is not null)
        {
            _netShService.EnableAutoConfig(_runningAdapter.Name);
            _netShService.ConnectInterface(_runningAdapter.Name, _runningAdapter.ProfileName);
        }
        this.Log().Debug("Hotspot stopped");
        _sourceAdapter = null;
    }

    // separate because of registry stuff
    private int _adapterIndex;
    public int AdapterIndex
    {
        get => _adapterIndex;
        set => SetAdapterIndex(value);
    }

    private int GetAdapterIndex()
    {
        var byteArr = _registryService.GetData(
            "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\icssvc\\Settings",
            "PreferredPublicInterface", 
            new byte[0]);
        // https://superuser.com/questions/1448054/choose-which-adapter-to-use-for-mobile-hotspot-on-windows-10
        // guid formed in 4-2-2-2-6 byte sequence, first 3 subseqs are reversed
        // new guid() seems to handle that for us? how nice!
        var guid = new Guid(byteArr);
        for (int i = 0; i < _adapters!.Count; i += 1)
        {
            if (_adapters[i].Id == guid)
            {
                return i;
            }
        }
        return -1;
    }

    private void SetAdapterIndex(int index)
    {
        if (!IsAdministrator)
        {
            throw new Exception("How did you do this?");
        }

        _registryService.SetData(
            "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\icssvc\\Settings",
            "PreferredPublicInterface",
            _adapters[index].Id.ToByteArray());
        _adapterIndex = GetAdapterIndex();
    }
}