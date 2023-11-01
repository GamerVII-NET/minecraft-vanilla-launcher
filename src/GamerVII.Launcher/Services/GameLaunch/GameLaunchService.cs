using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using CmlLib.Core.Version;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.Logger;
using Splat;

namespace GamerVII.Launcher.Services.GameLaunch;

public class GameLaunchService : IGameLaunchService
{
    private readonly ILoggerService _loggerService;
    private readonly CMLauncher _launcher;

    public event IGameLaunchService.ProgressChangedEventHandler? ProgressChanged;
    public event IGameLaunchService.FileChangedEventHandler? FileChanged;
    public event IGameLaunchService.LoadClientEventHandler? LoadClientEnded;

    public GameLaunchService(ILoggerService? loggerService = null)
    {
        _loggerService = loggerService ?? Locator.Current.GetService<ILoggerService>()!;

        System.Net.ServicePointManager.DefaultConnectionLimit = 256;

        var path = new MinecraftPath();
        _launcher = new CMLauncher(path);

        _launcher.ProgressChanged += (_, e) => ProgressChanged?.Invoke(((decimal)e.ProgressPercentage) / 100);
        _launcher.FileChanged += (e) => FileChanged?.Invoke(e.FileName);
    }

    public async Task<Process> LaunchClient(IGameClient client, IUser user, IStartupOptions startupOptions)
    {
        // var session = new MSession(user.Login, user.AccessToken, "uuid");
        var session = MSession.CreateOfflineSession(user.Login);

        var process = await _launcher.CreateProcessAsync(client.InstallationVersion, new MLaunchOption
        {
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

    public Task<IGameClient> LoadClient(IGameClient client)
    {
        var thread = new Thread(() => LoadClientFiles(client))
        {
            IsBackground = true
        };

        thread.Start();

        return Task.FromResult(client);
    }

    public async Task<IEnumerable<IMinecraftVersion>> GetAvailableVersions()
    {
        var versions = await _launcher.GetAllVersionsAsync();

        return versions.Select(c => new MinecraftVersion
            {
                Version = c.Name,
                MVersion = c
            })
            .OrderByDescending(c => c.MVersion.ReleaseTime);
    }

    private async void LoadClientFiles(IGameClient client)
    {
        try
        {
            var version = await _launcher.GetVersionAsync(client.Version);
            client.InstallationVersion = version.Id;

            switch (client.ModLoaderType)
            {
                case Models.Enums.ModLoaderType.Vanilla:
                    await _launcher.CheckAndDownloadAsync(version);
                    break;

                case Models.Enums.ModLoaderType.Forge:
                    await LoadForge(client);
                    break;

                case Models.Enums.ModLoaderType.Fabric:
                    break;
                case Models.Enums.ModLoaderType.LiteLoader:
                    break;
            }

            LoadClientEnded?.Invoke(client, true, "success");
        }
        catch (Exception ex)
        {
            LoadClientEnded?.Invoke(client, false, ex.Message);
        }
    }

    private async Task LoadForge(IGameClient client)
    {
        var forge = new MForge(_launcher);
        forge.FileChanged += (e) => FileChanged?.Invoke(e.FileName);
        forge.ProgressChanged += (_, e) => ProgressChanged?.Invoke(((decimal)e.ProgressPercentage) / 100);

        client.InstallationVersion = await forge.Install(client.Version);
    }
}
