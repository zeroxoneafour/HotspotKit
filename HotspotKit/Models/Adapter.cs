using System;
using Windows.Networking.Connectivity;
using Windows.Networking.NetworkOperators;

namespace HotspotKit.Models;

public class Adapter : IAdapter
{
    private readonly NetworkAdapter _adapter;
    private readonly NetShInfo _netShInfo;
    private NetworkOperatorTetheringManager _tetheringManager;
    public Adapter(NetworkAdapter networkAdapter, NetShInfo netShInfo)
    {
        _adapter = networkAdapter;
        _netShInfo = netShInfo;
        ReloadTetheringManager();
    }

    public Guid Id => _adapter.NetworkAdapterId;
    public string Name => _netShInfo.Name;
    public string ProfileName { get; private set; }

    private void ReloadTetheringManager()
    {
        var connectionProfile = _adapter.GetConnectedProfileAsync().GetAwaiter().GetResult();
        if (connectionProfile is null) throw new Exception("Network not connected");
        ProfileName = connectionProfile.ProfileName;
        _tetheringManager = NetworkOperatorTetheringManager
            .CreateFromConnectionProfile(connectionProfile, _adapter);

    }
    
    public void StartTethering()
    {
        _tetheringManager.StartTetheringAsync().GetAwaiter().GetResult();
    }
    
    public void StopTethering()
    {
        _tetheringManager.StopTetheringAsync().GetAwaiter().GetResult();
    }
}