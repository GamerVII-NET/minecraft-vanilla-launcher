using System;
using System.Runtime.InteropServices;

namespace GamerVII.Launcher.Services.System;

public class SystemService : ISystemService
{
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetPhysicallyInstalledSystemMemory(out ulong totalMemoryInKb);

    public ulong GetMaxAvailableRam()
    {
        if (GetPhysicallyInstalledSystemMemory(out var totalMemoryInKb))
        {
            return totalMemoryInKb / 1024;
        }
        return 0;
    }


}
