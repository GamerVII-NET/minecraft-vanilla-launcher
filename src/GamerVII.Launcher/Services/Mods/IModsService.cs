using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Mods;

namespace GamerVII.Launcher.Services.Mods;

public interface IModsService
{
    Task<IEnumerable<IMod>> GetModsAsync(IFilter<IFilterItem> filter, CancellationToken cancellationToken);
    Task<IEnumerable<IMinecraftVersion>> GetMinecraftVersionsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<IModCategory>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<IModVersion>> GetModVersions(string modSlug, CancellationToken cancellationToken);
    Task<IModVersion?> GetLatestVersionAsync(string modSlug, string clientVersion, CancellationToken cancellationToken);
    Task<IModVersion?> GetVersionAsync(string projectId, string versionId, CancellationToken cancellationToken);
    Task<IModInfo?> GetModInfoAsync(string modSlug, CancellationToken cancellationToken);
}
