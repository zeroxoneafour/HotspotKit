namespace HotspotKit.Models;

public interface IAdapter
{
    public string Id { get; }
    public string Name { get; }
    public string ProfileName { get; }
    public void StartTethering();
    public void StopTethering();
}