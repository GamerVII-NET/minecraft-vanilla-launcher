using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Enums;
using GamerVII.Launcher.Services.Mods;
using GamerVII.Launcher.Services.System;
using GamerVII.Launcher.ViewModels.Base;
using Modrinth.Api.Core.Filter;
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

                _filter = new ProjectFilter();
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
        /// Gets or sets the installed mods list
        /// </summary>
        public ObservableCollection<IMod> InstalledMods
        {
            get => _installedMods;
            set => this.RaiseAndSetIfChanged(ref _installedMods, value);
        }

        /// <summary>
        /// Gets or sets the to install mods list
        /// </summary>
        public ObservableCollection<IMod> ModsToInstall
        {
            get => _modsToInstall;
            set => this.RaiseAndSetIfChanged(ref _modsToInstall, value);
        }

        /// <summary>
        /// Gets or sets Selected mod
        /// </summary>
        public IMod? SelectedMod
        {
            get => _selectedMod;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedMod, value);
                LoadAdditionalModInfo();
            }
        }

        private async Task LoadAdditionalModInfo()
        {
            if (SelectedMod != null)
            {
                SelectedModInfo = await _modsService.GetModInfoAsync(SelectedMod.Slug, CancellationToken.None);
            }
        }

        /// <summary>
        /// Gets or sets Selected mod
        /// </summary>
        public IModInfo? SelectedModInfo
        {
            get => _selectedModInfo;
            set => this.RaiseAndSetIfChanged(ref _selectedModInfo, value);
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
            set
            {
                this.RaiseAndSetIfChanged(ref _searchText, value);
                _filter.Offset = 0;
            }
        }

        /// <summary>
        /// Gets or sets Filter
        /// </summary>
        public ProjectFilter SearchFilter
        {
            get => _filter;
            set
            {
                this.RaiseAndSetIfChanged(ref _filter, value);
                _filter.Offset = 0;
            }
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

        /// <summary>
        /// A flag that indicates whether the system is currently busy with a task.
        /// </summary>
        public bool IsDownloadingMods
        {
            get => _isDownloadingMods;
            set => this.RaiseAndSetIfChanged(ref _isDownloadingMods, value);
        }

        #endregion

        #region Private properties

        private IGameClient? _selectedClient;
        private ObservableCollection<IMod> _mods = new();
        private ObservableCollection<IMod> _installedMods = new();
        private ObservableCollection<IMod> _modsToInstall = new();
        private IMod? _selectedMod;
        private IModInfo? _selectedModInfo;
        private ObservableCollection<IMinecraftVersion> _minecraftVersions = new();
        private ObservableCollection<IModCategory> _modCategories = new();
        private readonly IModsService _modsService;
        private readonly ISystemService _systemService;
        private ProjectFilter _filter = new();
        private string _searchText;
        private string _minecraftVersionSearchText;
        private string _categorySearchText;
        private bool _isBusy;
        private bool _isDownloadingMods;

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

            var canRemove = this.WhenAnyValue(
                x => x.IsDownloadingMods,
                (isProcessing) => isProcessing == false
            );

            AddModToClientCommand = ReactiveCommand.CreateFromTask<IMod>(AddModToClient);
            RemoveModFromClientCommand = ReactiveCommand.CreateFromTask<IMod>(RemoveModFromClient, canRemove);
            SaveModsListCommand = ReactiveCommand.CreateFromTask(SaveModsList);
            ToggleCategoryCommand = ReactiveCommand.CreateFromTask<IModCategory>(ToggleCategory);
            LoadNextElementsCommand = ReactiveCommand.CreateFromTask(LoadNextMods);
            RefreshFilterCommand = ReactiveCommand.CreateFromTask(Refreshfilter);

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

        /// <summary>
        /// Command to remove mod from client
        /// </summary>
        public ICommand? RemoveModFromClientCommand { get; set; }

        /// <summary>
        /// Command to load next mods
        /// </summary>
        public ICommand LoadNextElementsCommand { get; set; }

        /// <summary>
        /// Command to refresh filter
        /// </summary>
        public ICommand RefreshFilterCommand { get; set; }

        /// <summary>
        /// Command to toggle enabled category filter
        /// </summary>
        public ICommand ToggleCategoryCommand { get; set; }

        /// <summary>
        /// Command to save mods list
        /// </summary>
        public ICommand SaveModsListCommand { get; set; }

        #endregion

        #region Private methods

        internal async void LoadData()
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

            _filter.Query = searchText;

            if (!string.IsNullOrWhiteSpace(_selectedClient?.Version))
            {
                _filter.AddFacet(ProjectFilterTypes.Version, _selectedClient.Version);
            }

            if (!string.IsNullOrWhiteSpace(_selectedClient?.ModLoaderName) && _selectedClient.ModLoaderType != ModLoaderType.Vanilla)
            {
                _filter.AddFacet(ProjectFilterTypes.Category, _selectedClient.ModLoaderName);
            }

            await LoadMods();

            IsBusy = false;
        }

        private async Task AddModToClient(IMod mod, CancellationToken cancellationToken)
        {
            if (!ModsToInstall.Contains(mod))
            {
                ModsToInstall.Add(mod);

                this.RaisePropertyChanged(nameof(ModsToInstall));
            }
        }

        private async Task RemoveModFromClient(IMod mod, CancellationToken cancellationToken)
        {
            IsDownloadingMods = true;

            if (ModsToInstall.Contains(mod))
            {
                ModsToInstall.Remove(mod);

                this.RaisePropertyChanged(nameof(ModsToInstall));
            }

            if (InstalledMods.Contains(mod))
            {
                InstalledMods.Remove(mod);

                var gameDirectory = await _systemService.GetGamePath();
                var minecraftVersion = await _modsService.GetLatestVersionAsync(mod.Slug, cancellationToken);


                minecraftVersion?.Files
                    .Select(c => new FileInfo(Path.Combine(gameDirectory, "mods", c)))
                    .ToList()
                    .ForEach(c =>
                    {
                        if (c.Exists) c.Delete();
                    });

                this.RaisePropertyChanged(nameof(InstalledMods));
            }
            IsDownloadingMods = false;
        }
        private async Task ToggleCategory(IModCategory mod, CancellationToken cancellationToken)
        {
            _filter.ToggleFacet(ProjectFilterTypes.Category, mod.Name);

            await LoadMods();
        }

        private async Task SaveModsList(CancellationToken token)
        {
            IsDownloadingMods = true;
            var modsToInstall = ModsToInstall.ToList();

            foreach (var mod in modsToInstall)
            {
                var modsFolder = Path.Combine(await _systemService.GetGamePath(), "mods");
                await _modsService.LoadModAsync(modsFolder, mod.Slug, token);
                ModsToInstall.Remove(mod);
                InstalledMods.Add(mod);
            }

            IsDownloadingMods = false;
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

        private async Task LoadNextMods(CancellationToken arg)
        {
            _filter.Offset += _filter.Limit;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                _filter.Query = SearchText;
            }

            if (!string.IsNullOrWhiteSpace(_selectedClient?.Version))
            {
                _filter.AddFacet(ProjectFilterTypes.Version, _selectedClient.Version);
            }

            if (!string.IsNullOrWhiteSpace(_selectedClient?.ModLoaderName) && _selectedClient.ModLoaderType != ModLoaderType.Vanilla)
            {
                _filter.AddFacet(ProjectFilterTypes.Category, _selectedClient.ModLoaderName.ToLower());
            }

            var mods = await _modsService.GetModsAsync(_filter, token);

            Mods.AddRange(mods);
        }


        private async Task Refreshfilter()
        {
            SearchFilter = new ProjectFilter();
            SearchText = string.Empty;
            DoSearch(SearchText);
        }

        #endregion
    }
}
