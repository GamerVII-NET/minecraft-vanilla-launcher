using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading.Tasks;
using GamerVII.Launcher.Services.LocalStorage;
using Mono.Unix;
using Splat;

namespace GamerVII.Launcher.Services.System;

public class SystemService : ISystemService
{
    private readonly ILocalStorageService _localStorage;

    public SystemService(ILocalStorageService? localStorage = null)
    {
        _localStorage = localStorage
                        ?? Locator.Current.GetService<ILocalStorageService>()
                        ?? throw new Exception($"{nameof(ILocalStorageService)} not registered!");
    }

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
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Convert.ToUInt64(GetUnixAvailableRam());
        }

        return 0;
    }

    public async Task<string> GetInstallationDirectory()
    {

        var appDirectory = await _localStorage.GetAsync<string>("InstallationPath");

        if (!string.IsNullOrEmpty(appDirectory))
        {
            return appDirectory;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            appDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            appDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            appDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        if (string.IsNullOrEmpty(appDirectory) || CanCreateDirectory(appDirectory) == false)
        {
            appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppDomain.CurrentDomain.BaseDirectory;
        }

        await _localStorage.SetAsync("InstallationPath", appDirectory);

        return appDirectory;
    }

    public async Task<string> GetGamePath()
    {
        return Path.GetFullPath(Path.Combine(await GetInstallationDirectory(), "gamervii-launcher"));
    }

    public async Task SetInstallationDirectory(string appDirectory)
    {
        await _localStorage.SetAsync("InstallationPath", appDirectory);
    }

    private static bool CanCreateDirectory(string path)
    {
        try
        {
            var tempDirectory = Path.Combine(path, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            Directory.Delete(tempDirectory);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
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

        using (var process = Process.Start(info))
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
