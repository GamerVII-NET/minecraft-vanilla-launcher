using GamerVII.Launcher.Extensions;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Enums;
using GamerVII.Launcher.Services.Client;
using GamerVII.Launcher.Services.GameLaunch;
using GamerVII.Launcher.Tests.Models;
using GamerVII.Launcher.ViewModels;
using GamerVII.Launcher.ViewModels.Pages;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher.Tests;

public class GameClientsTests
{
    private MainWindowViewModel _mainViewModel;
    private AddClientPageViewModel _addClientViewModel;
    private IGameLaunchService _gameLaunchService;

    [SetUp]
    public void Setup()
    {
        ServiceRegister.RegisterServices();

        _mainViewModel = new MainWindowViewModel();


        _addClientViewModel = _mainViewModel.GetPageViewModelByType<AddClientPageViewModel>() as AddClientPageViewModel
                              ?? throw new Exception($"{nameof(AddClientPageViewModel)} not found");


        _gameLaunchService = Locator.Current.GetService<IGameLaunchService>() ??
            throw new Exception($"{nameof(IGameLaunchService)} not found");
    }

    [Test, Order(1)]
    public async Task GetRandomMinecraftVersionTest()
    {
        var randomMinecraftVersion = await GetRandomVMinecraftVersion();

        Assert.That(randomMinecraftVersion, Is.Not.Null);
    }

    private async Task<IMinecraftVersion> GetRandomVMinecraftVersion()
    {
        Random random = new();

        var versions = (await _gameLaunchService.GetAvailableVersions()).ToList();

        var randomMinecraftVersion = versions[random.Next(0, versions.Count - 1)];
        return randomMinecraftVersion;
    }

    [Test, Order(2)]
    public async Task CanCreateClientTest()
    {
        await CreateMinecraftClient();

        var canCreate = _addClientViewModel.SaveClientCommand.CanExecute(_addClientViewModel.NewGameClient);

        Assert.That(canCreate, Is.True);
    }

    [Test, Order(3)]
    public async Task CreateClientTest()
    {

        await CreateMinecraftClient();

        _addClientViewModel.SaveClientCommand.Execute(null);

        Assert.That(_mainViewModel.SidebarViewModel.ServersListViewModel.GameClients.Last(), Is.EqualTo(_addClientViewModel.NewGameClient));
    }



    private async Task CreateMinecraftClient()
    {
        var randomMinecraftVersion = await GetRandomVMinecraftVersion();

        _addClientViewModel.NewGameClient = new GameClient
        {
            Version = randomMinecraftVersion.Version,
            Name = "Server",
            Description = "Description for server",
            InstallationVersion = randomMinecraftVersion.Version,
            ModLoaderType = ModLoaderType.Vanilla,
            Image = null
        };
    }
}
