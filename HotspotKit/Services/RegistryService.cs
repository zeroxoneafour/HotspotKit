using System;
using Microsoft.Win32;

namespace HotspotKit.Services;

public class RegistryService
{
    public T GetData<T>(string key, string value, T defaultValue)
    {
        return (T)(Registry.GetValue(key, value, defaultValue)
            ?? throw new Exception("Value " + value + " not found"));
    }

    public void SetData<T>(string key, string value, T data)
    {
        Registry.SetValue(key, value, data);
    }
}