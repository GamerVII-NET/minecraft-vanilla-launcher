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

        // servers.Add(new GameClient
        // {
        //     Name = $"Название сервера",
        //     Image = null,
        //     Description = "Описание",
        //     Version = "1.7.10",
        //     ModLoaderType = ModLoaderType.Vanilla
        // });

        return await Task.FromResult(servers);
    }
}
