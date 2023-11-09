using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Mods;
using GamerVII.Launcher.Models.Mods.Modrinth;
using Newtonsoft.Json;

namespace GamerVII.Launcher.Services.Mods;

public class ModrinthModsService : IModsService
{
    private readonly HttpClient _httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(30),
        BaseAddress = new Uri("https://api.modrinth.com")
    };

    private IEnumerable<IModCategory> _categoriesStorage = Enumerable.Empty<MCategory>();
    private IEnumerable<IMinecraftVersion> _minecraftVersionsStorage = Enumerable.Empty<IMinecraftVersion>();

    public ModrinthModsService()
    {
        // _httpClient.DefaultRequestHeaders.Add("User-Agent", "GamerVII-NET/minecraft-vanilla-launcher/1.1.0 (launcher.recloud.tech)");
    }

    public async Task<IEnumerable<IMod>> GetModsAsync(IFilter<IFilterItem> filter, CancellationToken cancellationToken)
    {
        filter.AddIfNotExists(new ModrinthFilterItem("project_type", "mod"));

        var uri = filter.GetParametersString("/v2/search");

        var request = await _httpClient.GetAsync(uri, cancellationToken);

        if (!request.IsSuccessStatusCode)
            return Enumerable.Empty<IMod>();

        var data = await request.Content.ReadAsStringAsync(cancellationToken);

        var rootData = JsonConvert.DeserializeObject<MSearch>(data);

        return rootData?.Hits.Select(c => new ClientMod
        {
            Author = c.Author,
            Description = c.Description,
            Downloads = c.Downloads,
            Follows = c.Follows,
            Gallery = c.Gallery,
            Title = c.Title,
            DateCreated = c.DateCreated,
            DateModified = c.DateModified,
            IconUrl = c.IconUrl,
            Slug = c.Slug,
            LatestVersion = c.LatestVersion
        }) ?? Enumerable.Empty<IMod>();
    }

    public async Task<IEnumerable<IMinecraftVersion>> GetMinecraftVersionsAsync(CancellationToken cancellationToken)
    {
        if (_minecraftVersionsStorage.Any()) return _minecraftVersionsStorage;

        var request = await _httpClient.GetAsync("/v2/tag/game_version", cancellationToken);

        if (!request.IsSuccessStatusCode)
            return Enumerable.Empty<IMinecraftVersion>();

        var data = await request.Content.ReadAsStringAsync(cancellationToken);

        _minecraftVersionsStorage = JsonConvert.DeserializeObject<IEnumerable<MGameVersion>>(data)
                                    ?? Enumerable.Empty<IMinecraftVersion>();

        return _minecraftVersionsStorage;
    }

    public async Task<IEnumerable<IModCategory>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        if (_categoriesStorage.Any()) return _categoriesStorage;

        var request = await _httpClient.GetAsync("/v2/tag/category", cancellationToken);

        if (!request.IsSuccessStatusCode)
            return Enumerable.Empty<IModCategory>();

        var data = await request.Content.ReadAsStringAsync(cancellationToken);

        _categoriesStorage = JsonConvert.DeserializeObject<IEnumerable<MCategory>>(data)
                             ?? Enumerable.Empty<IModCategory>();

        return _categoriesStorage;
    }

    public async Task<IEnumerable<IModVersion>> GetModVersions(string modSlug,
        CancellationToken cancellationToken)
    {
        var request = await _httpClient.GetAsync($"/v2/project/{modSlug}/version", cancellationToken);

        if (!request.IsSuccessStatusCode)
            return Enumerable.Empty<IModVersion>();

        var data = await request.Content.ReadAsStringAsync(cancellationToken);

        var modeVersions = JsonConvert.DeserializeObject<IEnumerable<MVersion>>(data)
                           ?? Enumerable.Empty<IModVersion>();

        return modeVersions;
    }

    public async Task<IModVersion?> GetLatestVersionAsync(string modSlug, string clientVersion, CancellationToken cancellationToken)
    {
        var versions = await GetModVersions(modSlug, cancellationToken);

        var modVersions = versions.ToList();

        return modVersions.OfType<MVersion>().OrderByDescending(c => c.DatePublished).FirstOrDefault(c => c.GameVersions.Contains(clientVersion) && c.VersionType == "release")
               ?? modVersions.OfType<MVersion>().OrderByDescending(c => c.DatePublished).FirstOrDefault(c => c.GameVersions.Contains(clientVersion));
    }

    public async Task<IModVersion?> GetVersionAsync(string projectId, string versionId, CancellationToken cancellationToken)
    {
        var versions = await GetModVersions(projectId, cancellationToken);

        return versions.OfType<MVersion>().MaxBy(c => c.DatePublished);
    }

    public async Task<IModInfo?> GetModInfoAsync(string modSlug, CancellationToken cancellationToken)
    {
        var request = await _httpClient.GetAsync($"/v2/project/{modSlug}", cancellationToken);

        if (!request.IsSuccessStatusCode)
            return null;

        var data = await request.Content.ReadAsStringAsync(cancellationToken);

        var modInfo = JsonConvert.DeserializeObject<MProjectInfo>(data);

        if (modInfo != null)
            return new ClientModInfo
            {
                Slug = modSlug,
                Name = modInfo.Title,
                ShortDescription = modInfo.Description,
                FullDescription = modInfo.Body
            };

        return null;
    }
}
