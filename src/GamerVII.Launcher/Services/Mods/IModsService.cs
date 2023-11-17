using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Client;
using Modrinth.Api.Core.Filter;

namespace GamerVII.Launcher.Services.Mods;

public interface IModsService
{
    Task<IEnumerable<IMod>> GetModsAsync(ProjectFilter filter, CancellationToken cancellationToken);
    Task<IEnumerable<IMinecraftVersion>> GetMinecraftVersionsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<IModCategory>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<IModInfo?> GetModInfoAsync(string selectedModSlug, CancellationToken cancellationToken);
    Task LoadModAsync(string modsFolder, string slug, CancellationToken token);
    Task<IModVersion?> GetLatestVersionAsync(string modSlug, CancellationToken cancellationToken);
}
