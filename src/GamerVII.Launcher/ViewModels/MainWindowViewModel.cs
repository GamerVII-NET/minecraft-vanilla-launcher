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
using GamerVII.Launcher.ViewModels.Pages;
using GamerVII.Notification.Avalonia;
using Splat;

namespace GamerVII.Launcher.ViewModels
{
    /// <summary>
    /// View model class for the main window, derived from ViewModelBase.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Public properties

        /// <summary>
        /// Gets the Sidebar ViewModel.
        /// </summary>
        public SidebarViewModel SidebarViewModel { get; }

        /// <summary>
        /// Gets the Notification manager.
        /// </summary>
        public INotificationMessageManager Manager { get; } = new NotificationMessageManager();

        #region Current user

        /// <summary>
        /// Gets or sets the currently authorized user.
        /// </summary>
        public IUser? User
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
        public PageViewModelBase? CurrentPage
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
        /// Command to open a link in a browser.
        /// </summary>
        public ICommand OpenLinkCommand { get; }

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
        private IUser? _user;
        private PageViewModelBase? _currentPage;

        // Array of available page view models.
        private readonly PageViewModelBase[] _pages =
        {
            new AuthPageViewModel(),
            new ProfilePageViewModel(),
            new SettingsPageViewModel(),
            new ModsPageViewModel(),
            new AddClientPageViewModel(),
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MainWindowViewModel class.
        /// </summary>
        public MainWindowViewModel(
            IGameLaunchService? gameLaunchService = null,
            IAuthService? authService = null,
            ILocalStorageService? storageService = null,
            ILoggerService? loggerService = null)
        {
            SidebarViewModel = new SidebarViewModel();

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

            var canLaunch = this.WhenAnyValue(
                x => x.IsProcessing, x => x.SidebarViewModel.ServersListViewModel.SelectedClient,
                (isProcessing, gameClient) =>
                    isProcessing == false &&
                    gameClient != null
            );

            var canViewMods = this.WhenAnyValue(
                x => x.IsProcessing, x => x.SidebarViewModel.ServersListViewModel.SelectedClient,
                (isProcessing, gameClient) => gameClient != null && isProcessing == false
            );

            // Set up commands with corresponding actions and conditions.
            SidebarViewModel.LogoutCommand = ReactiveCommand.CreateFromTask(Logout);

            SidebarViewModel.ServersListViewModel.AddClientCommand =
                ReactiveCommand.Create(() => OpenPage<AddClientPageViewModel>());

            LaunchGameCommand = ReactiveCommand.CreateFromTask(LaunchGame, canLaunch);

            SidebarViewModel.ServersListViewModel.SelectedServerChanged += ResetPage;

            SidebarViewModel.OpenProfilePageCommand = ReactiveCommand.Create(() =>
                OpenPage<ProfilePageViewModel>(c => ((ProfilePageViewModel)c).User = User));

            SettingsClientCommand = ReactiveCommand.Create(() =>
                OpenPage<SettingsPageViewModel>(c => ((SettingsPageViewModel)c).User = User));

            ModsListCommand = ReactiveCommand.Create(() =>
                    OpenPage<ModsPageViewModel>(c =>
                        ((ModsPageViewModel)c).SelectClient = SidebarViewModel.ServersListViewModel.SelectedClient!)
                , canViewMods);

            OpenLinkCommand = ReactiveCommand.Create((string url) => OpenLink(url));

            // Subscribe to events from the game launch service to update processing information.
            SubscribeToEvents();

            // Load user data and set the appropriate initial page based on user status.
            LoadData();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saves the application settings to local storage.
        /// </summary>
        public async Task SaveSettings()
        {
            if (GetPageViewModelByType<SettingsPageViewModel>() is SettingsPageViewModel settings)
            {
                await _storageService.SetAsync("Settings", new LocalSettings
                {
                    WindowWidth = settings.WindowWidth,
                    WindowHeight = settings.WindowHeight,
                    IsFullScreen = settings.IsFullScreen,
                    MemorySize = settings.MemorySize,
                    ConnectionLimit = settings.ConnectionLimit
                });
            }

            ResetPage();
        }

        /// <summary>
        /// Resets the current page by setting it to null.
        /// </summary>
        public void ResetPage()
        {
            CurrentPage = null!;
        }

        /// <summary>
        /// Gets a page view model by its type.
        /// </summary>
        /// <typeparam name="T">The type of the page view model to retrieve.</typeparam>
        /// <returns>The page view model of the specified type, or null if not found.</returns>
        public PageViewModelBase? GetPageViewModelByType<T>()
        {
            var type = typeof(T);

            var page = _pages.FirstOrDefault(c => c.GetType() == type);

            return page;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Opens the specified URL in the default web browser.
        /// </summary>
        /// <param name="url">The URL to open.</param>
        private static void OpenLink(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        /// <summary>
        /// Subscribes to events related to the game launch service.
        /// </summary>
        private void SubscribeToEvents()
        {
            // Event handlers for file changes and progress changes.
            _gameLaunchService.FileChanged += (fileName) => { LoadingFile = fileName; };
            _gameLaunchService.ProgressChanged += (percentage) => { LoadingPercentage = percentage; };

            // Event handler for the client loading process.
            _gameLaunchService.LoadClientEnded += async (client, isSuccess, message) =>
            {
                if (!isSuccess)
                {
                    await CancelUiProcessing();

                    if (!string.IsNullOrEmpty(message))
                    {
                        await _loggerService.Log(message);
                        await _loggerService.Log(message);

                        Manager
                            .CreateMessage(true, "#151515", "Error", message)
                            .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
                            .Queue();
                    }
                }

                // Create a new game client and launch the client process.
                var settings = GetPageViewModelByType<SettingsPageViewModel>() as SettingsPageViewModel
                               ?? throw new Exception("Settings not found");

                if (User == null) return;

                var process = await _gameLaunchService.LaunchClient(client, User, new StartupOptions
                {
                    ScreenWidth = settings.WindowWidth,
                    ScreenHeight = settings.WindowHeight,
                    FullScreen = settings.IsFullScreen,
                    MaximumRamMb = settings.MemorySize,
                    MinimumRamMb = settings.MemorySize,
                });

                // Event handler for the client process exit.
                process.Exited += async (sender, e) =>
                {
                    await CancelUiProcessing();

                    process.Dispose();
                };
            };
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
            User = await _authService.GetAuthorizedUser();

            if (!User.IsLogin) OpenPage<AuthPageViewModel>();

            // Obtain references to various view models.
            var authViewModel = GetPageViewModelByType<AuthPageViewModel>() as AuthPageViewModel
                                ?? throw new Exception(nameof(AuthPageViewModel) + " not registered!");

            var profileViewModel = GetPageViewModelByType<ProfilePageViewModel>() as ProfilePageViewModel
                                   ?? throw new Exception(nameof(ProfilePageViewModel) + " not registered!");

            var settingsViewModel = GetPageViewModelByType<SettingsPageViewModel>() as SettingsPageViewModel
                                    ?? throw new Exception(nameof(SettingsPageViewModel) + " not registered!");

            var addServerViewModel = GetPageViewModelByType<AddClientPageViewModel>() as AddClientPageViewModel
                                     ?? throw new Exception(nameof(AddClientPageViewModel) + " not registered!");

            var modsPageViewModel = GetPageViewModelByType<ModsPageViewModel>() as ModsPageViewModel
                                    ?? throw new Exception(nameof(ModsPageViewModel) + " not registered!");

            // Load user settings and update view model properties.
            if (await _storageService.GetAsync<LocalSettings>("Settings") is { } settings)
            {
                settingsViewModel.WindowWidth = settings.WindowWidth == 0 ? 900 : settings.WindowWidth;
                settingsViewModel.WindowHeight = settings.WindowHeight == 0 ? 600 : settings.WindowHeight;
                settingsViewModel.MemorySize = settings.MemorySize == 0 ? 1024 : settings.MemorySize;
                settingsViewModel.IsFullScreen = settings.IsFullScreen;
                settingsViewModel.ConnectionLimit = settings.ConnectionLimit;
            }

            // Set up navigation commands for each view model.
            profileViewModel.GoToMainPageCommand = ReactiveCommand.Create(ResetPage);
            settingsViewModel.GoToMainPageCommand = ReactiveCommand.CreateFromTask(SaveSettings);
            addServerViewModel.GoToMainPageCommand = ReactiveCommand.Create(ResetPage);
            modsPageViewModel.GoToMainPageCommand = ReactiveCommand.Create(ResetPage);
            addServerViewModel.SaveClientCommand = ReactiveCommand.Create(() => AddGameClient(addServerViewModel.NewGameClient));

            // Subscribe to the Authorized event from the authentication view model.
            authViewModel.Authorized += OnAuthorized;
        }

        /// <summary>
        /// Handles the Authorized event when the user logs in.
        /// </summary>
        /// <param name="user">The authorized user.</param>
        private void OnAuthorized(IUser user)
        {
            if (!user.IsLogin) return;

            User = user;

            ResetPage();
        }

        /// <summary>
        /// Opens the specified page view model and sets it as the current page.
        /// </summary>
        /// <typeparam name="T">The type of the page view model to open.</typeparam>
        /// <param name="action">An optional action to perform on the opened page view model.</param>
        public void OpenPage<T>(Func<PageViewModelBase, object?>? action = null)
        {
            var page = GetPageViewModelByType<T>();

            var index = _pages.IndexOf(page);

            if (index == -1) return;

            CurrentPage = _pages[index];

            action?.Invoke(CurrentPage);
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

                if (SidebarViewModel.ServersListViewModel.SelectedClient != null)
                {
                    var client = await _gameLaunchService.LoadClient(SidebarViewModel.ServersListViewModel.SelectedClient);
                }
            }
            catch (Exception e)
            {
                await _loggerService.Log(e.Message, e);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Adds a game client to the list of available game clients.
        /// </summary>
        /// <param name="gameClient">The game client to add.</param>
        private void AddGameClient(IGameClient? gameClient)
        {
            if (gameClient == null)
            {
                Manager
                    .CreateMessage(true, "#151515", "Error", "Failed to retrieve client information")
                    .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
                    .Queue();
                return;
            }

            if (string.IsNullOrWhiteSpace(gameClient.Name))
            {
                Manager
                    .CreateMessage(true, "#D03E3E", "Error", "Specify a client name")
                    .Dismiss().WithDelay(TimeSpan.FromSeconds(2))
                    .Queue();
                return;
            }

            if (string.IsNullOrEmpty(gameClient.Version))
            {
                Manager
                    .CreateMessage(true, "#D03E3E", "Error", "Select a game client version!")
                    .Dismiss().WithDelay(TimeSpan.FromSeconds(2))
                    .Queue();
                return;
            }

            SidebarViewModel.ServersListViewModel.GameClients.Add(gameClient);
            SidebarViewModel.ServersListViewModel.SelectedClient = gameClient;

            _storageService.SetAsync("Clients", SidebarViewModel.ServersListViewModel.GameClients);

            ResetPage();
        }

        /// <summary>
        /// Cancels UI processing and sets the IsProcessing flag to false.
        /// </summary>
        private async Task CancelUiProcessing()
        {
            await Dispatcher.UIThread.InvokeAsync(() => IsProcessing = false);
        }

        #endregion

    }
}
