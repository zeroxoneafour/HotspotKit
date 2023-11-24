using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HotspotKit.Models;
using Microsoft.VisualBasic.Logging;
using Splat;

namespace HotspotKit.Services;

public class NetShService : IEnableLogger
{
    private StreamReader _stdout;
    private StreamWriter _stdin;
    public NetShService()
    {
        var info = new ProcessStartInfo();
        info.FileName = "netsh.exe";
        info.CreateNoWindow = true;
        info.RedirectStandardInput = true;
        info.RedirectStandardOutput = true;

        var process = new Process();
        process.StartInfo = info;
        process.Start();
        
        _stdout = process.StandardOutput;
        _stdin = process.StandardInput;
    }

    public NetShInfo GetInfo(string guid)
    {
        _stdin.WriteLine("wlan show interfaces");
        var output = new List<string>();
        int index = 0;
        string lastOutput = "";
        while (!lastOutput.Contains(guid))
        {
            lastOutput = _stdout.ReadLine() ?? throw new Exception("Adapter not found");
            index += 1;
            output.Add(lastOutput);
        }

        var nameLine = output[index - 3];
        var name = nameLine[(nameLine.LastIndexOf(':') + 2)..];
        return new NetShInfo(name);
    }

    public void DisconnectInterface(string intf)
    {
        _stdin.WriteLine("wlan disconnect interface=\"" + intf + "\"");
    }
    
    public void ConnectInterface(string intf, string profile)
    {
        _stdin.WriteLine("wlan connect name=\"" + profile + "\" interface=\"" + intf + "\"");
    }

    public void DisableAutoConfig(string intf)
    {
        _stdin.WriteLine("wlan set autoconfig enabled=no interface=\"" + intf + "\"");
    }
    
    public void EnableAutoConfig(string intf)
    {
        _stdin.WriteLine("wlan set autoconfig enabled=yes interface=\"" + intf + "\"");
    }
}