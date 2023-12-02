using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Client;
using Modrinth.Api;
using Modrinth.Api.Core.Filter;
using Modrinth.Api.Models.Projects;

namespace GamerVII.Launcher.Services.Mods;

public class ModrinthModsService : IModsService
{
    private readonly ModrinthApi _modrinthApi = new();

    public async Task<IEnumerable<IMod>> GetModsAsync(ProjectFilter filter, CancellationToken cancellationToken)
    {
        var mods = await _modrinthApi.Mods.FindAsync<ModProject>(filter, cancellationToken);

        return mods.Hits.Select(c => new ClientMod
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
        });
    }

    public async Task<IEnumerable<IMinecraftVersion>> GetMinecraftVersionsAsync(CancellationToken cancellationToken)
    {
        var versions = await _modrinthApi.Other.GetMinecraftVersionsAsync(cancellationToken);

        return versions.Select(c => new MinecraftVersion
        {
            Version = c.Version,
            VersionType = c.VersionType
        });
    }

    public async Task<IEnumerable<IModCategory>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var versions = await _modrinthApi.Other.GetCategoriesAsync(cancellationToken);

        return versions.Select(c => new ModCategory
        {
            Name = c.Name,
            Type = c.ProjectType
        });
    }

    public async Task<IModInfo?> GetModInfoAsync(string selectedModSlug, CancellationToken token)
    {
        var modProject = await _modrinthApi.Mods.FindAsync<ModProject>(selectedModSlug, token);

        return new ClientModInfo
        {
            Name = modProject.Title,
            Slug = modProject.Slug,
            FullDescription = modProject.Body,
            ShortDescription = modProject.Description
        };
    }

    public async Task LoadModAsync(string modsFolder, string slug, string loaderName, CancellationToken token)
    {
        var modVersion = await _modrinthApi.Mods.GetLastVersionAsync(slug, loaderName, token);

        if (modVersion != null)
        {
            await _modrinthApi.Mods.DownloadAsync(modsFolder, modVersion, loaderName, true, token);
        }
    }

    public async Task<IModVersion?> GetLatestVersionAsync(string modSlug, string loaderName, CancellationToken cancellationToken)
    {
        var version = await _modrinthApi.Mods.GetLastVersionAsync(modSlug, loaderName, cancellationToken);

        if (version != null)
            return new ModVersion
            {
                Name = version.Name,
                Files = version.Files.Select(c => c.Filename)
            };

        return null;
    }
}
