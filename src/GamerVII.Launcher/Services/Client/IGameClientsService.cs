using System.Collections.Generic;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Client;

namespace GamerVII.Launcher.Services.Client;

/// <summary>
/// Represents a service for managing game clients.
/// </summary>
public interface IGameClientService
{
    /// <summary>
    /// Retrieves a collection of game clients asynchronously.
    /// </summary>
    /// <returns>An asynchronous operation that yields a collection of IGameClient instances.</returns>
    Task<IEnumerable<IGameClient>> GetClientsAsync();
}
