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

        for (var i = 0; i < 2; i++)
        {
            servers.Add(new GameClient
            {
                Name = $"Сервер-{i}",
                Image = null,
                Description = "Просто проект майнкрафт, без модов. Не знаю что ты тут ищешь",
                Version = "1.7.10",
                ModLoaderType = ModLoaderType.Vanilla
            });
        }

        return await Task.FromResult(servers);
    }
}
