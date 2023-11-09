using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Mods;
using GamerVII.Launcher.Models.Mods.Modrinth;
using GamerVII.Launcher.Services.Mods;
using GamerVII.Launcher.Services.System;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher.ViewModels.Pages
{
    /// <summary>
    /// View model class for the profile page, derived from PageViewModelBase.
    /// </summary>
    public class ModsPageViewModel : PageViewModelBase
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the selected game client.
        /// </summary>
        public IGameClient? SelectClient
        {
            get => _selectedClient;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedClient, value);

                _filter = new ModrinthFilter<IFilterItem>();
            }
        }

        /// <summary>
        /// Gets or sets the mods list
        /// </summary>
        public ObservableCollection<IMod> Mods
        {
            get => _mods;
            set => this.RaiseAndSetIfChanged(ref _mods, value);
        }

        /// <summary>
        /// Gets or sets the minecraft versions list
        /// </summary>
        public ObservableCollection<IMinecraftVersion> MinecraftVersions
        {
            get => _minecraftVersions;
            set => this.RaiseAndSetIfChanged(ref _minecraftVersions, value);
        }

        /// <summary>
        /// Gets or sets the mod categories list
        /// </summary>
        public ObservableCollection<IModCategory> ModCategories
        {
            get => _modCategories;
            set => this.RaiseAndSetIfChanged(ref _modCategories, value);
        }

        /// <summary>
        /// Gets or sets the search text
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        /// <summary>
        /// Gets or sets the minecraft version search text
        /// </summary>
        public string MinecraftVersionSearchText
        {
            get => _minecraftVersionSearchText;
            set => this.RaiseAndSetIfChanged(ref _minecraftVersionSearchText, value);
        }

        /// <summary>
        /// Gets or sets the category search text
        /// </summary>
        public string CategorySearchText
        {
            get => _categorySearchText;
            set => this.RaiseAndSetIfChanged(ref _categorySearchText, value);
        }

        /// <summary>
        /// A flag that indicates whether the system is currently busy with a task.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        #endregion

        #region Private properties

        private IGameClient? _selectedClient;
        private ObservableCollection<IMod> _mods = new();
        private ObservableCollection<IMinecraftVersion> _minecraftVersions = new();
        private ObservableCollection<IModCategory> _modCategories = new();
        private readonly IModsService _modsService;
        private readonly ISystemService _systemService;
        private IFilter<IFilterItem> _filter = new ModrinthFilter<IFilterItem>();
        private string _searchText;
        private string _minecraftVersionSearchText;
        private string _categorySearchText;
        private bool _isBusy;

        private CancellationTokenSource modsListTokenSource = new();
        private CancellationToken token;

        private CancellationTokenSource filterTokenSource = new();
        private CancellationToken filterToken;

        #endregion

        #region Constructors

        public ModsPageViewModel(
            IModsService? modsService = null,
            ISystemService? systemService = null)
        {
            _systemService = systemService
                             ?? Locator.Current.GetService<ISystemService>()
                             ?? throw new Exception($"{nameof(ISystemService)} not registered!");
            ;

            _modsService = modsService
                           ?? Locator.Current.GetService<IModsService>()
                           ?? throw new Exception($"{nameof(IModsService)} not registered!");

            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoSearch!);

            this.WhenAnyValue(x => x.MinecraftVersionSearchText)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoLoadMinecraftVersions!);

            this.WhenAnyValue(x => x.CategorySearchText)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoLoadCategories!);

            AddModToClientCommand = ReactiveCommand.CreateFromTask<IMod>(AddModToClient);

            RxApp.MainThreadScheduler.Schedule(LoadData);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to navigate to the main page.
        /// </summary>
        public ICommand? GoToMainPageCommand { get; set; }

        /// <summary>
        /// Command to add mod to client
        /// </summary>
        public ICommand? AddModToClientCommand { get; set; }

        #endregion

        #region Private methods

        private async void LoadData()
        {
            filterTokenSource = new CancellationTokenSource();
            filterToken = filterTokenSource.Token;

            await LoadMinecraftVersions(string.Empty);
            await LoadCategories(string.Empty);
        }

        private async Task LoadMinecraftVersions(string version)
        {
            var versions = await _modsService.GetMinecraftVersionsAsync(token);
            versions = versions.Where(c => c.VersionType == "release");

            if (!string.IsNullOrWhiteSpace(version))
                versions = versions.Where(c => c.Version.Contains(version));

            MinecraftVersions = new ObservableCollection<IMinecraftVersion>(versions);
        }

        private async Task LoadCategories(string name)
        {
            var categories = await _modsService.GetCategoriesAsync(filterToken);
            categories = categories.Where(c => c.Type == "mod");

            if (!string.IsNullOrWhiteSpace(name))
                categories = categories.Where(c => c.Name.Contains(name));

            ModCategories = new ObservableCollection<IModCategory>(categories);
        }

        private async Task LoadMods()
        {
            modsListTokenSource = new CancellationTokenSource();
            token = modsListTokenSource.Token;

            var mods = await _modsService.GetModsAsync(_filter, token);

            Mods = new ObservableCollection<IMod>(mods);
        }

        private async void DoLoadMinecraftVersions(string searchText)
        {
            IsBusy = true;

            await LoadMinecraftVersions(searchText);

            IsBusy = false;
        }

        private async void DoLoadCategories(string searchText)
        {
            IsBusy = true;

            await LoadCategories(searchText);

            IsBusy = false;
        }

        private async void DoSearch(string searchText)
        {
            IsBusy = true;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                _filter.Query = SearchText;
            }

            if (!string.IsNullOrWhiteSpace(_selectedClient?.Version))
            {
                _filter.AddIfNotExists(new ModrinthFilterItem("versions", _selectedClient.Version));
            }

            await LoadMods();

            IsBusy = false;
        }

        private async Task AddModToClient(IMod mod, CancellationToken cancellationToken)
        {
            IsBusy = true;

            var localPath = await _systemService.GetGamePath();

            var modsDirectory = Path.Combine(localPath, "mods");

            if (_selectedClient != null &&
                await _modsService.GetLatestVersionAsync(mod.Slug, _selectedClient.Version, cancellationToken) is
                    MVersion latestVersion)
            {
                foreach (var file in latestVersion.Files)
                {
                    await DownloadFileAsync(file.Url, modsDirectory, file.Filename, cancellationToken);
                }

                foreach (var dependency in latestVersion.Dependencies)
                {
                    var version =
                        await _modsService.GetVersionAsync(dependency.ProjectId, dependency.VersionId, CancellationToken.None) as MVersion;

                    foreach (var file in version.Files)
                    {
                        await DownloadFileAsync(file.Url, modsDirectory, file.Filename, CancellationToken.None);
                    }


                    // await DownloadFileAsync(file.Url, modsDirectory, file.Filename, cancellationToken);
                }
            }

            IsBusy = false;
        }

        private async Task DownloadFileAsync(string fileUrl, string targetFolderPath, string fileName,
            CancellationToken cancellationToken = default)
        {
            using var httpClient = new HttpClient();
            using var response =
                await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to download the file. Status code: {response.StatusCode}");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var fullPath = Path.Combine(targetFolderPath, fileName);

            await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fileStream, 81920, cancellationToken);
        }

        #endregion
    }
}
