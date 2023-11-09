using GamerVII.Launcher.Models.Enums;

namespace GamerVII.Launcher.Models.Client;

internal class GameClient : IGameClient
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string InstallationVersion { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public object? Image { get; set; }
    public string ModLoaderName { get; set; } = null!;
    public ModLoaderType ModLoaderType { get; set; }
}
