using DynamicData;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.Auth;
using GamerVII.Launcher.Services.GameLaunch;
using GamerVII.Launcher.Services.LocalStorage;
using GamerVII.Launcher.Services.Logger;
using Splat;

namespace GamerVII.Launcher.ViewModels
{
    /// <summary>
    /// View model class for the main window, derived from ViewModelBase.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        // Sidebar view model for the main window.
        public SidebarViewModel SidebarViewModel { get; }

        #region Public properties

        #region Current user

        /// <summary>
        /// Gets or sets the currently authorized user.
        /// </summary>
        public IUser User
        {
            get => _user;
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }

        #endregion

        #region Processing

        /// <summary>
        /// Gets or sets a value indicating whether an operation is in progress.
        /// </summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            set => this.RaiseAndSetIfChanged(ref _isProcessing, value);
        }

        #endregion

        #region Current page

        /// <summary>
        /// Gets or sets the current page view model displayed in the main window.
        /// </summary>
        public PageViewModelBase CurrentPage
        {
            get => _currentPage;
            private set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        #endregion

        #region Processing file name

        /// <summary>
        /// Gets or sets the name of the file currently being processed.
        /// </summary>
        public string LoadingFile
        {
            get => _loadingFile;
            set => this.RaiseAndSetIfChanged(ref _loadingFile, value);
        }

        #endregion

        #region Processing percentage

        /// <summary>
        /// Gets or sets the percentage of the processing progress.
        /// </summary>
        public decimal LoadingPercentage
        {
            get => _loadingPercentage;
            set => this.RaiseAndSetIfChanged(ref _loadingPercentage, value);
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to launch the game.
        /// </summary>
        public ICommand LaunchGameCommand { get; }

        /// <summary>
        /// Command to open the client settings page.
        /// </summary>
        public ICommand SettingsClientCommand { get; }

        /// <summary>
        /// Command to view the list of mods.
        /// </summary>
        public ICommand ModsListCommand { get; }

        #endregion

        #region Private variables

        private readonly IAuthService _authService;
        private readonly IGameLaunchService _gameLaunchService;
        private readonly ILocalStorageService _storageService;
        private readonly ILoggerService _loggerService;

        private bool _isProcessing = false;
        private string _loadingFile = string.Empty;
        private decimal _loadingPercentage = 0;
        private IUser _user;
        private PageViewModelBase _currentPage;

        // Array of available page view models.
        private readonly PageViewModelBase[] Pages =
        {
            new AuthPageViewModel(),
            new ProfilePageViewModel(),
            new ClientSettingsPageViewModel(),
            new ModsPageViewModel(),
        };

        #endregion


        /// <summary>
        /// Initializes a new instance of the MainWindowViewModel class.
        /// </summary>
        /// <param name="gameLaunchService">An optional IGameLaunchService implementation for game launching.</param>
        /// <param name="authService">An optional IAuthService implementation for user authentication.</param>
        /// <param name="loggerService">An optional ILoggerService implementation for logging messages.</param>
        public MainWindowViewModel(
            IGameLaunchService? gameLaunchService = null,
            IAuthService? authService = null,
            ILocalStorageService? storageService = null,
            ILoggerService? loggerService = null)
        {
            // Initialize the SidebarViewModel for the main window.
            SidebarViewModel = new SidebarViewModel();

            // Set the provided or default implementations for game launch service, authentication service, and logger service.
            _gameLaunchService = gameLaunchService
                                 ?? Locator.Current.GetService<IGameLaunchService>()
                                 ?? throw new Exception($"{nameof(IGameLaunchService)} not registered");

            _authService = authService
                           ?? Locator.Current.GetService<IAuthService>()
                           ?? throw new Exception($"{nameof(IAuthService)} not registered");

            _loggerService = loggerService
                             ?? Locator.Current.GetService<ILoggerService>()
                             ?? throw new Exception($"{nameof(ILoggerService)} not registered");

            _storageService = storageService
                              ?? Locator.Current.GetService<ILocalStorageService>()
                              ?? throw new Exception($"{nameof(ILocalStorageService)} not registered");


            // Define conditions for enabling certain commands based on view model properties.
            var canLaunch = this.WhenAnyValue(
                x => x.IsProcessing, x => x.SidebarViewModel.ServersListViewModel.SelectedClient,
                (isProcessing, gameClient) =>
                    isProcessing == false &&
                    gameClient != null
            );

            var canViewMods = this.WhenAnyValue(
                x => x.IsProcessing, x => x.SidebarViewModel.ServersListViewModel.SelectedClient,
                (isProcessing, gameClient) => gameClient != null
            );

            // Set up commands with corresponding actions and conditions.
            SidebarViewModel.LogoutCommand = ReactiveCommand.CreateFromTask(Logout);
            LaunchGameCommand = ReactiveCommand.CreateFromTask(LaunchGame, canLaunch);
            SidebarViewModel.ServersListViewModel.SelectedServerChanged += ResetPage;

            SidebarViewModel.OpenProfilePageCommand = ReactiveCommand.Create(() =>
                OpenPage<ProfilePageViewModel>(c => ((ProfilePageViewModel)c).User = User));

            SettingsClientCommand =
                ReactiveCommand.Create(
                    () => OpenPage<ClientSettingsPageViewModel>(
                        c => ((ClientSettingsPageViewModel)c).User = User),
                    canLaunch);

            ModsListCommand = ReactiveCommand.Create(() =>
            {
                OpenPage<ModsPageViewModel>(c =>
                    ((ModsPageViewModel)c).SelectClient = SidebarViewModel.ServersListViewModel.SelectedClient!);
            }, canViewMods);

            // Subscribe to events from the game launch service to update processing information.
            _gameLaunchService.FileChanged += (fileName) =>
            {
                LoadingFile = fileName;
                Console.WriteLine(LoadingFile);
            };
            _gameLaunchService.ProgressChanged += (percentage) =>
            {
                LoadingPercentage = percentage;
                Console.WriteLine(LoadingPercentage);
            };

            _gameLaunchService.LoadClientEnded += async (client, isSuccess, message) =>
            {
                try
                {
                    if (isSuccess)
                    {
                        var settings =
                            GetPageViewModelByType<ClientSettingsPageViewModel>() as ClientSettingsPageViewModel ??
                            throw new Exception("Settings not found");

                        var process = await _gameLaunchService.LaunchClient(client, User, new StartupOptions
                        {
                            ScreenWidth = settings.WindowWidth,
                            ScreenHeight = settings.WindowHeight,
                            FullScreen = settings.IsFullScreen,
                            MaximumRamMb = settings.MemorySize,
                            MinimumRamMb = settings.MemorySize,
                        });

                        process.Exited += async (sender, e) =>
                        {
                            await CancelUiProcessing();

                            process.Dispose();
                        };
                    }
                    else
                    {
                        await CancelUiProcessing();
                        Console.WriteLine(message);
                    }
                }
                catch (Exception ex)
                {
                    _loggerService.Log(ex.Message, ex);
                }
            };

            // Load user data and set the appropriate initial page based on user status.
            LoadData();
        }


        /// <summary>
        /// Handles the logout operation.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        private async Task Logout()
        {
            await _authService.OnLogout();
            User = null;

            OpenPage<AuthPageViewModel>();
        }

        /// <summary>
        /// Loads initial data, sets the user, and determines the initial page to display.
        /// </summary>
        private async void LoadData()
        {
            User = await _authService.GetAuthorizedUser() ?? new User { Login = String.Empty, Password = string.Empty };

            if (!User.IsLogin) OpenPage<AuthPageViewModel>();

            var authViewModel = Pages.FirstOrDefault(c => c.GetType() == typeof(AuthPageViewModel)) as AuthPageViewModel
                                ?? throw new Exception(nameof(AuthPageViewModel) + " not found");

            var profileViewModel =
                Pages.FirstOrDefault(c => c.GetType() == typeof(ProfilePageViewModel)) as ProfilePageViewModel
                ?? throw new Exception(nameof(ProfilePageViewModel) + " not found");

            var settingsViewModel =
                Pages.FirstOrDefault(c => c.GetType() == typeof(ClientSettingsPageViewModel)) as
                    ClientSettingsPageViewModel
                ?? throw new Exception(nameof(ClientSettingsPageViewModel) + " not found");

            var modsPageViewModel =
                Pages.FirstOrDefault(c => c.GetType() == typeof(ModsPageViewModel)) as ModsPageViewModel
                ?? throw new Exception(nameof(ModsPageViewModel) + " not found");

            if (await _storageService.GetAsync<LocalSettings>("Settings") is { } settings)
            {
                settingsViewModel.WindowWidth = settings.WindowWidth == 0 ? 900 : settings.WindowWidth;
                settingsViewModel.WindowHeight = settings.WindowHeight == 0 ? 600 : settings.WindowWidth;
                settingsViewModel.MemorySize = settings.MemorySize == 0 ? 1024 : settings.MemorySize;
                settingsViewModel.IsFullScreen = settings.IsFullScreen;
            }

            profileViewModel.GoToMainPageCommand = ReactiveCommand.Create(ResetPage);
            settingsViewModel.GoToMainPageCommand = ReactiveCommand.CreateFromTask(SaveSettings);
            modsPageViewModel.GoToMainPageCommand = ReactiveCommand.Create(ResetPage);


            authViewModel.Authorized += async (user) =>
            {
                if (!user.IsLogin) return;

                User = user;
                ResetPage();
            };
        }

        private async Task SaveSettings()
        {
            if (GetPageViewModelByType<ClientSettingsPageViewModel>() is ClientSettingsPageViewModel settings)
            {
                await _storageService.SetAsync("Settings", new LocalSettings
                {
                    WindowWidth = settings.WindowWidth,
                    WindowHeight = settings.WindowHeight,
                    IsFullScreen = settings.IsFullScreen,
                    MemorySize = settings.MemorySize
                });
            }

            ResetPage();
        }

        /// <summary>
        /// Resets the current page by setting it to null.
        /// </summary>
        private void ResetPage()
        {
            CurrentPage = null!;
        }

        /// <summary>
        /// Opens the specified page view model and sets it as the current page.
        /// </summary>
        /// <typeparam name="T">The type of the page view model to open.</typeparam>
        private void OpenPage<T>(Func<PageViewModelBase, object>? action = null)
        {
            var page = GetPageViewModelByType<T>();

            var index = Pages.IndexOf(page);

            if (index != -1)
            {
                CurrentPage = Pages[index];

                action?.Invoke(CurrentPage);
            }
        }

        private PageViewModelBase? GetPageViewModelByType<T>()
        {
            var type = typeof(T);

            var page = Pages.FirstOrDefault(c => c.GetType() == type);

            return page;
        }

        /// <summary>
        /// Launches the game based on the selected game client.
        /// </summary>
        /// <param name="arg">The cancellation token for the async task.</param>
        /// <returns>An asynchronous task.</returns>
        private async Task LaunchGame(CancellationToken arg)
        {
            try
            {
                IsProcessing = true;

                IGameClient client =
                    await _gameLaunchService.LoadClient(SidebarViewModel.ServersListViewModel.SelectedClient);
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex.Message);
                _loggerService.Log(ex.StackTrace);
            }
        }

        private async Task CancelUiProcessing()
        {
            await Dispatcher.UIThread.InvokeAsync(() => IsProcessing = false);
        }
    }
}
