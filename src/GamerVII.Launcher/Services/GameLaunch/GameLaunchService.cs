using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.Logger;
using GamerVII.Launcher.Services.System;
using Splat;

namespace GamerVII.Launcher.Services.GameLaunch;

public class GameLaunchService : IGameLaunchService
{
    public int ConnectionLimit { get; set; } = 128;

    private readonly ISystemService _systemService;
    private readonly ILoggerService _loggerService;
    private CMLauncher? _launcher;

    public event IGameLaunchService.ProgressChangedEventHandler? ProgressChanged;
    public event IGameLaunchService.FileChangedEventHandler? FileChanged;
    public event IGameLaunchService.LoadClientEventHandler? LoadClientEnded;

    public GameLaunchService(ILoggerService? loggerService = null, ISystemService? systemService = null)
    {
        _systemService = systemService
                         ?? Locator.Current.GetService<ISystemService>()
                         ?? throw new Exception($"{nameof(ISystemService)} not registered");

        _loggerService = loggerService
                         ?? Locator.Current.GetService<ILoggerService>()
                         ?? throw new Exception($"{nameof(ILoggerService)} not registered");

        ServicePointManager.DefaultConnectionLimit = ConnectionLimit;
    }

    public async Task<CMLauncher> InitMinecraftLauncherAsync(CancellationToken cancellationToken)
    {
        var gamePath = await _systemService.GetGamePath();

        var path = new CustomMinecraftPath(gamePath);

        _launcher = new CMLauncher(path);

        _launcher.ProgressChanged += (_, e) => { ProgressChanged?.Invoke(e.ProgressPercentage); };

        _launcher.FileChanged += (e) =>
        {
            if (e.FileName != null)
                FileChanged?.Invoke(e.FileName);
        };

        return _launcher;
    }

    public async Task<Process> LaunchClientAsync(IGameClient client, IUser user, IStartupOptions startupOptions,
        CancellationToken cancellationToken)
    {
        _launcher ??= await InitMinecraftLauncherAsync(cancellationToken);

        // var session = new MSession(user.Login, user.AccessToken, "uuid");
        var session = MSession.CreateOfflineSession(user.Login);

        var process = await _launcher.CreateProcessAsync(client.InstallationVersion, new MLaunchOption
        {
            JavaPath = _launcher.GetDefaultJavaPath()?.Replace("javaw.exe", "java.exe"),
            MinimumRamMb = startupOptions.MinimumRamMb,
            MaximumRamMb = startupOptions.MaximumRamMb,
            FullScreen = startupOptions.FullScreen,
            ScreenHeight = startupOptions.ScreenHeight,
            ScreenWidth = startupOptions.ScreenWidth,
            ServerIp = startupOptions.ServerIp,
            ServerPort = startupOptions.ServerPort,
            Session = session,
        }, false);

        process.EnableRaisingEvents = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                _loggerService.Log(e.Data);
        };
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
                _loggerService.Log(e.Data);
        };

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        return process;
    }

    public async Task<IGameClient> LoadClientAsync(IGameClient client, CancellationToken cancellationToken)
    {
        await LoadClientFilesAsync(client, cancellationToken);

        return client;

    }

    public async Task<IEnumerable<IMinecraftVersion>> GetAvailableVersionsAsync(CancellationToken cancellationToken)
    {
        try
        {

            _launcher ??= await InitMinecraftLauncherAsync(cancellationToken);

            var versions = await _launcher.GetAllVersionsAsync();

            return versions.Select(c =>
                    new MinecraftVersion
                    {
                        Version = c.Name,
                        MVersion = c
                    })
                .OrderByDescending(c => c.MVersion?.ReleaseTime);
        }
        catch (Exception exception)
        {
            await _loggerService.Log(exception.Message, exception);
        }

        return Enumerable.Empty<IMinecraftVersion>();
    }

    private async Task LoadClientFilesAsync(IGameClient client, CancellationToken cancellationToken)
    {
        try
        {
            _launcher ??= await InitMinecraftLauncherAsync(cancellationToken);

            var version = await _launcher.GetVersionAsync(client.Version);

            client.InstallationVersion = version.Id;

            switch (client.ModLoaderType)
            {
                case Models.Enums.ModLoaderType.Vanilla:

                    if (!File.Exists(_launcher.MinecraftPath.GetVersionJarPath(client.Version)))
                        await _launcher.CheckAndDownloadAsync(version);

                    break;

                case Models.Enums.ModLoaderType.Forge:
                    await LoadForgeAsync(client, cancellationToken);
                    break;

                case Models.Enums.ModLoaderType.Fabric:
                    throw new NotImplementedException();
                case Models.Enums.ModLoaderType.LiteLoader:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            LoadClientEnded?.Invoke(client, true, "success");
        }
        catch (Exception ex)
        {
            LoadClientEnded?.Invoke(client, false, ex.Message);
        }
    }

    private async Task LoadForgeAsync(IGameClient client, CancellationToken cancellationToken)
    {
        _launcher ??= await InitMinecraftLauncherAsync(cancellationToken);

        var forge = new MForge(_launcher);

        forge.FileChanged += (e) =>
        {
            if (e.FileName != null) FileChanged?.Invoke(e.FileName);
        };

        forge.ProgressChanged += (_, e) => ProgressChanged?.Invoke(e.ProgressPercentage);

        client.InstallationVersion = await forge.Install(client.Version);
    }
}
