using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.GameLaunch;
using GamerVII.Launcher.Services.System;
using GamerVII.Launcher.ViewModels.Base;
using GamerVII.Launcher.Views;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher.ViewModels.Pages;

/// <summary>
/// View model class for the client settings page, derived from PageViewModelBase.
/// </summary>
public class SettingsPageViewModel : PageViewModelBase
{
    private readonly IGameLaunchService? _gameLaunchService;


    public int MemorySize
    {
        get => _memorySize;
        set
        {
            if (value % 8 == 0)
            {
                this.RaiseAndSetIfChanged(ref _memorySize, value);
            }
        }
    }
    public int MaxMemorySize
    {
        get => _maxMemorySize;
        set => this.RaiseAndSetIfChanged(ref _maxMemorySize, value);
    }
    public int WindowWidth
    {
        get => _windowWidth;
        set => this.RaiseAndSetIfChanged(ref _windowWidth, value);
    }
    public int WindowHeight
    {
        get => _windowHeight;
        set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
    }

    public bool IsFullScreen
    {
        get => _isFullScreen;
        set => this.RaiseAndSetIfChanged(ref _isFullScreen, value);
    }
    public IUser? User
    {
        get => _user;
        set => this.RaiseAndSetIfChanged(ref _user, value);
    }
    public string InstallationFolderPath
    {
        get => _installationFolderPath;
        set => this.RaiseAndSetIfChanged(ref _installationFolderPath, value);
    }


    private int _memorySize = 1024;
    private int _maxMemorySize = 1024;
    private int _windowWidth = 900;
    private int _windowHeight = 600;
    private bool _isFullScreen;
    private IUser? _user;
    private string _installationFolderPath = string.Empty;

    private readonly ISystemService _systemService;
    /// <summary>
    /// Command to navigate to the main page.
    /// </summary>
    public ICommand? GoToMainPageCommand { get; set; }
    public ICommand SelectFolderCommand { get; }


    public SettingsPageViewModel(ISystemService? systemService = null, IGameLaunchService? gameLaunchService = null)
    {
        _gameLaunchService = gameLaunchService
                             ?? Locator.Current.GetService<IGameLaunchService>()
                             ?? throw new Exception($"{nameof(IGameLaunchService)} not registered");

        _systemService = systemService
                         ?? Locator.Current.GetService<ISystemService>()
                         ?? throw new Exception($"{nameof(ISystemService)} not registered");

        MaxMemorySize = Convert.ToInt32(_systemService.GetMaxAvailableRam());

        SelectFolderCommand = ReactiveCommand.CreateFromTask(OnSelectFolder);

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }


    private async void LoadData()
    {
        InstallationFolderPath = await _systemService.GetGamePath();
    }

    private async Task OnSelectFolder()
    {
        var options = new FolderPickerOpenOptions()
        {
            Title = "Выберите папку для установки",
            AllowMultiple = false
        };

        var appLifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;


        if (appLifetime?.MainWindow is MainWindow mainWindow)
        {
            var result = await mainWindow.StorageProvider.OpenFolderPickerAsync(options);

            if (result.Any())
            {

                await _systemService.SetInstallationDirectory(InstallationFolderPath);

                InstallationFolderPath =  await _systemService.GetGamePath();

                if (_gameLaunchService is GameLaunchService launchService)
                {
                    await launchService.InitMinecraftLauncher();
                }
            }
        }
    }
}
