using System;

namespace HotspotKit.Models;

public interface IAdapter
{
    public Guid Id { get; }
    public string Name { get; }
    public string ProfileName { get; }
    public void StartTethering();
    public void StopTethering();
}