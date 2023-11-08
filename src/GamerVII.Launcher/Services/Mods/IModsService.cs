using System.Collections.Generic;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Mods;
using GamerVII.Launcher.Models.Mods.Modrinth;

namespace GamerVII.Launcher.Services.Mods;

public interface IModsService
{
    Task<IEnumerable<IMod>> GetModsListAsync(IFilter<IFilterItem> filter);
}
