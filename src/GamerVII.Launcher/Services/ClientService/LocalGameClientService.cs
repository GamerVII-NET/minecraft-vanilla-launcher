using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamerVII.Launcher.Services.ClientService;

internal class LocalGameClientService : IGameClientService
{
    public async Task<IEnumerable<IGameClient>> GetClientsAsync()
    {
        List<GameClient> servers = new List<GameClient>();

        for (int i = 0; i < 2; i++)
        {
            servers.Add(new GameClient
            {
                Name = $"Сервер-{i}",
                Image = null,
                Description = "Просто проект майнкрафт, без модов. Не знаю что ты тут ищешь",
                Version = "1.7.10",
                ModLoaderType = ModLoaderType.Forge
            });
        }

        return await Task.FromResult(servers);
    }
}
