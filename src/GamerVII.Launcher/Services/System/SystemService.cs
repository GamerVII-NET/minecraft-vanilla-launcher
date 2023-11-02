using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Mono.Unix;

namespace GamerVII.Launcher.Services.System;

public class SystemService : ISystemService
{
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetPhysicallyInstalledSystemMemory(out ulong totalMemoryInKb);

    public ulong GetMaxAvailableRam()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (GetPhysicallyInstalledSystemMemory(out var totalMemoryInKb))
            {
                return totalMemoryInKb / 1024;
            }
        }else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Convert.ToUInt64(GetUnixAvailableRam());
        }

        return 0;
    }


    private ulong GetUnixAvailableRam()
    {
        string? output;

        var info = new ProcessStartInfo("free -m")
        {
            FileName = "/bin/bash",
            Arguments = "-c \"free -m\"",
            RedirectStandardOutput = true
        };

        using(var process = Process.Start(info))
        {
            output = process?.StandardOutput.ReadToEnd();
        }

        if (string.IsNullOrEmpty(output))
            return 0;

        var lines = output.Split("\n");
        var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

        return Convert.ToUInt64(memory[1]);
    }
}
