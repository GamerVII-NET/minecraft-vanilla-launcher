using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Enums;
using GamerVII.Launcher.Services.GameLaunch;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher.ViewModels.Pages;

public class AddClientPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Command to navigate to the main page.
    /// </summary>
    public ICommand? GoToMainPageCommand { get; set; }

    /// <summary>
    /// Command to add new client
    /// </summary>
    public ICommand? SaveClientCommand { get; set; }

    public IGameClient NewGameClient
    {
        get => _newGameClient;
        set => this.RaiseAndSetIfChanged(ref _newGameClient, value);
    }

    public ObservableCollection<IMinecraftVersion> MinecraftVersions
    {
        get => _minecraftVersions;
        set => this.RaiseAndSetIfChanged(ref _minecraftVersions, value);
    }

    public IMinecraftVersion? SelectedVersion
    {
        get => _selectedVersion;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedVersion, value);

            if (SelectedVersion != null)
            {
                var newClient = new GameClient
                {
                    Version = SelectedVersion.Version,
                    Name = string.IsNullOrWhiteSpace(_newGameClient.Name) || FindVersion(_newGameClient.Name) != null
                        ? SelectedVersion.Version
                        : _newGameClient.Name,
                    ModLoaderType = SelectedVersion.Version.Contains("Forge")
                        ? ModLoaderType.Forge
                        : ModLoaderType.Vanilla,
                    Description = _newGameClient.Description
                };

                NewGameClient = newClient;
            }
        }
    }

    public string? SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    private IGameClient _newGameClient = new GameClient();
    private ObservableCollection<IMinecraftVersion> _minecraftVersions = new();
    private IMinecraftVersion? _selectedVersion;
    private readonly IGameLaunchService _gameLaunchService;
    private string? _searchText;
    private bool _isBusy;

    /// <summary>
    /// Initializes a new instance of the ProfilePageViewModel class.
    /// </summary>
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

    private async void LoadData()
    {
        await LoadVersions();
    }

    private async Task LoadVersions()
    {
        var versions = await _gameLaunchService.GetAvailableVersions();

        MinecraftVersions = new ObservableCollection<IMinecraftVersion>(versions);
    }

    private async void DoSearch(string text)
    {
        IsBusy = true;
        MinecraftVersions.Clear();


        if (!string.IsNullOrWhiteSpace(text))
        {
            text = text.Replace(",", ".");

            var versions = await _gameLaunchService.GetAvailableVersions();

            versions = versions.Where(c => c.Version.Contains(text));

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
}
