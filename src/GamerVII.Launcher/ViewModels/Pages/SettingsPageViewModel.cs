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

namespace GamerVII.Launcher.ViewModels.Pages
{
    /// <summary>
    /// View model class for the client settings page, derived from PageViewModelBase.
    /// </summary>
    public class SettingsPageViewModel : PageViewModelBase
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the allocated memory size for the client.
        /// </summary>
        public int MemorySize
        {
            get => _memorySize;
            set
            {
                if (value % 8 == 0)
                    this.RaiseAndSetIfChanged(ref _memorySize, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum available memory size for the client.
        /// </summary>
        public int MaxMemorySize
        {
            get => _maxMemorySize;
            set => this.RaiseAndSetIfChanged(ref _maxMemorySize, value);
        }

        /// <summary>
        /// Gets or sets the window width of the client.
        /// </summary>
        public int WindowWidth
        {
            get => _windowWidth;
            set => this.RaiseAndSetIfChanged(ref _windowWidth, value);
        }

        /// <summary>
        /// Gets or sets the window height of the client.
        /// </summary>
        public int WindowHeight
        {
            get => _windowHeight;
            set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
        }

        /// <summary>
        /// Gets or sets the maximum connection limit for the client.
        /// </summary>
        public int ConnectionLimit
        {
            get => _connectionLimit;
            set
            {
                this.RaiseAndSetIfChanged(ref _connectionLimit, value);

                _gameLaunchService.ConnectionLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the client runs in fullscreen mode.
        /// </summary>
        public bool IsFullScreen
        {
            get => _isFullScreen;
            set => this.RaiseAndSetIfChanged(ref _isFullScreen, value);
        }

        /// <summary>
        /// Gets or sets the currently authenticated user.
        /// </summary>
        public IUser? User
        {
            get => _user;
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }

        /// <summary>
        /// Gets or sets the installation folder path for the game client.
        /// </summary>
        public string InstallationFolderPath
        {
            get => _installationFolderPath;
            set => this.RaiseAndSetIfChanged(ref _installationFolderPath, value);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to navigate to the main page.
        /// </summary>
        public ICommand? GoToMainPageCommand { get; set; }

        /// <summary>
        /// Command to open a folder picker for selecting the installation directory.
        /// </summary>
        public ICommand SelectFolderCommand { get; }

        #endregion

        #region Private variables

        private int _memorySize = 1024;
        private int _maxMemorySize = 1024;
        private int _windowWidth = 900;
        private int _windowHeight = 600;
        private int _connectionLimit = 128;
        private bool _isFullScreen;
        private IUser? _user;
        private string _installationFolderPath = string.Empty;

        private readonly IGameLaunchService _gameLaunchService;
        private readonly ISystemService _systemService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SettingsPageViewModel class.
        /// </summary>
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

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the initial data and settings.
        /// </summary>
        private async void LoadData()
        {
            InstallationFolderPath = await _systemService.GetGamePath();
        }

        /// <summary>
        /// Handles the folder selection process.
        /// </summary>
        private async Task OnSelectFolder()
        {
            var options = new FolderPickerOpenOptions()
            {
                Title = "Select the installation folder",
                AllowMultiple = false
            };

            var appLifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

            // ToDo: Remove access to a visual element from the ViewModel
            if (appLifetime?.MainWindow is MainWindow mainWindow)
            {
                var result = await mainWindow.StorageProvider.OpenFolderPickerAsync(options);

                if (result.Any())
                {
                    await _systemService.SetInstallationDirectory(InstallationFolderPath);

                    InstallationFolderPath = await _systemService.GetGamePath();
                }
            }

            if (_gameLaunchService is GameLaunchService launchService)
            {
                await launchService.InitMinecraftLauncher();
            }
        }

        #endregion
    }
}
