using System;
using System.Windows.Input;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.System;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher.ViewModels.Pages;

/// <summary>
/// View model class for the client settings page, derived from PageViewModelBase.
/// </summary>
public class SettingsPageViewModel : PageViewModelBase
{

    /// <summary>
    /// Command to navigate to the main page.
    /// </summary>
    public ICommand? GoToMainPageCommand { get; set; }


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
    public IUser User
    {
        get => _user;
        set => this.RaiseAndSetIfChanged(ref _user, value);
    }


    private int _memorySize = 1024;
    private int _maxMemorySize = 1024;
    private int _windowWidth = 900;
    private int _windowHeight = 600;
    private bool _isFullScreen = false;
    private IUser _user = null;

    private readonly ISystemService _systemService;

    public SettingsPageViewModel(ISystemService? systemService = null)
    {
        _systemService = systemService
                         ?? Locator.Current.GetService<ISystemService>()
                         ?? throw new Exception($"{nameof(ISystemService)} not registered");

        MaxMemorySize = Convert.ToInt32(_systemService.GetMaxAvailableRam());
    }

}
