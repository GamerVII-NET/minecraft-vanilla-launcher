using GamerVII.Launcher.Extensions;
using GamerVII.Launcher.Models.Mods;
using GamerVII.Launcher.Models.Mods.Modrinth;
using GamerVII.Launcher.Services.Mods;
using Splat;

namespace GamerVII.Launcher.Tests;

public class ModsLoaderTests
{
    private IModsService _modsService;
    private ModrinthFilter<IFilterItem> filter;

    [SetUp]
    public void Setup()
    {
        ServiceRegister.RegisterServices();

        _modsService = Locator.Current.GetService<IModsService>() ?? throw new Exception($"{nameof(IModsService)} not registered");

        filter = new ModrinthFilter<IFilterItem>();

    }

    [Test, Order(1)]
    public async Task GetModsListTest()
    {
        filter.Query = "Industrinal craft";
        filter.Limit = 5;
        filter.Offset = 0;

        filter.Add(new ModrinthFilterItem("versions", "1.12.2"));

        var mods = await _modsService.GetModsAsync(filter, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(mods, Is.Not.Null);
        });
    }

    [Test, Order(2)]
    public async Task GetMinecraftVersionsListTest()
    {

        var versions = await _modsService.GetMinecraftVersionsAsync(CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(versions, Is.Not.Null);
        });
    }

    [Test, Order(2)]
    public async Task GetCategoriesListTest()
    {

        var categories = await _modsService.GetCategoriesAsync(CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(categories, Is.Not.Null);
        });
    }
}
