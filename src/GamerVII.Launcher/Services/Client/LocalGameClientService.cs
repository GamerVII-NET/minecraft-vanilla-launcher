using System.Collections.Generic;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Enums;

namespace GamerVII.Launcher.Services.Client;

public class LocalGameClientService : IGameClientService
{
    public async Task<IEnumerable<IGameClient>> GetClientsAsync()
    {
        var servers = new List<GameClient>();

        return await Task.FromResult(servers);
    }
}
