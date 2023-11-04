using System.Threading.Tasks;

namespace GamerVII.Launcher.Services.System;

public interface ISystemService
{
    ulong GetMaxAvailableRam();
    Task<string> GetInstallationDirectory();
    Task<string> GetGamePath();
    Task SetInstallationDirectory(string appDirectory);
}
