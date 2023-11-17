using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Enums;
using GamerVII.Launcher.Services.GameLaunch;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher.ViewModels.Pages
{
    public class AddClientPageViewModel : PageViewModelBase
    {
        #region Public properties

        #region Game client

        /// <summary>
        /// Gets or sets the new game client.
        /// </summary>
        public IGameClient NewGameClient
        {
            get => _newGameClient;
            set => this.RaiseAndSetIfChanged(ref _newGameClient, value);
        }

        #endregion

        #region Minecraft versions list

        /// <summary>
        /// Gets or sets the collection of available Minecraft versions.
        /// </summary>
        public ObservableCollection<IMinecraftVersion> MinecraftVersions
        {
            get => _minecraftVersions;
            set => this.RaiseAndSetIfChanged(ref _minecraftVersions, value);
        }

        #endregion

        #region Search minecraft version text

        /// <summary>
        /// Gets or sets the search text for filtering Minecraft versions.
        /// </summary>
        public string? SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        #endregion

        #region Is busy value

        /// <summary>
        /// Gets or sets a value indicating whether the page is busy.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        #endregion

        #region Game loader

        /// <summary>
        /// Gets or sets a game loader
        /// </summary>
        public ObservableCollection<IGameLoader> GameLoaders
        {
            get => _gameLoaders;
            set
            {
                this.RaiseAndSetIfChanged(ref _gameLoaders, value);

                SelectedGameLoader = value?.FirstOrDefault();
            }
        }

        #endregion

        #region Selected loader

        /// <summary>
        /// Gets or sets a selected game loader
        /// </summary>
        public IGameLoader? SelectedGameLoader
        {
            get => _selectedGameLoader;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedGameLoader, value);

                if (value == null) return;

                NewGameClient.ModLoaderType = value.LoaderType;
                NewGameClient.ModLoaderName = value.Name;
            }
        }

        #endregion

        #region SelectedVersion

        /// <summary>
        /// Gets or sets the selected Minecraft version.
        /// </summary>
        public IMinecraftVersion? SelectedVersion
        {
            get => _selectedVersion;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedVersion, value);

                if (SelectedVersion == null) return;

                var newClient = new GameClient
                {
                    Version = SelectedVersion.Version,
                    Name = string.IsNullOrWhiteSpace(_newGameClient.Name) || FindVersion(_newGameClient.Name) != null
                        ? SelectedVersion.Version
                        : _newGameClient.Name,
                    ModLoaderType =
                        SelectedVersion.Version
                            .Contains("Forge") //ToDo: Add a proper implementation for installing the loader type.
                            ? ModLoaderType.Forge
                            : ModLoaderType.Vanilla,
                    Description = _newGameClient.Description
                };

                NewGameClient = newClient;
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to navigate to the main page.
        /// </summary>
        public ICommand? GoToMainPageCommand { get; set; }

        /// <summary>
        /// Command to add a new client.
        /// </summary>
        public ICommand? SaveClientCommand { get; set; }

        #endregion

        #region Private variables

        private IGameClient _newGameClient = new GameClient();
        private ObservableCollection<IMinecraftVersion> _minecraftVersions = new();
        private IGameLoader? _selectedGameLoader;

        private ObservableCollection<IGameLoader> _gameLoaders = new()
        {
            new GameLoader
            {
                Name = "Vanilla",
                LoaderType = ModLoaderType.Vanilla
            },
            new GameLoader
            {
                Name = "Forge",
                LoaderType = ModLoaderType.Forge
            },
            new GameLoader
            {
                Name = "Quilt",
                LoaderType = ModLoaderType.Quilt
            }
        };

        private IMinecraftVersion? _selectedVersion;
        private readonly IGameLaunchService _gameLaunchService;
        private string? _searchText;
        private bool _isBusy;

        #endregion

        #region Constructors

        public AddClientPageViewModel(IGameLaunchService? gameLaunchService = null)
        {
            _gameLaunchService = gameLaunchService
                                 ?? Locator.Current.GetService<IGameLaunchService>()
                                 ?? throw new Exception($"{nameof(IGameLaunchService)} not registered");

            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoSearch!);

            RxApp.MainThreadScheduler.Schedule(LoadData);
        }

        #endregion

        #region Private methods

        private async void LoadData()
        {
            await LoadVersions();
        }

        private async Task LoadVersions()
        {
            var versions = await _gameLaunchService.GetAvailableVersionsAsync(CancellationToken.None);

            MinecraftVersions = new ObservableCollection<IMinecraftVersion>(versions);
        }

        private async void DoSearch(string text)
        {
            IsBusy = true;
            MinecraftVersions.Clear();

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.Replace(",", ".");

                var versions = await _gameLaunchService.GetAvailableVersionsAsync(CancellationToken.None);

                versions = versions.Where(c => c.Version.Contains(text));

                await Task.Delay(500);
                MinecraftVersions = new ObservableCollection<IMinecraftVersion>(versions);
            }
            else
            {
                await LoadVersions();
            }

            IsBusy = false;
        }

        private IMinecraftVersion? FindVersion(string version)
        {
            return MinecraftVersions.FirstOrDefault(c => c.Version == version);
        }

        #endregion
    }
}
