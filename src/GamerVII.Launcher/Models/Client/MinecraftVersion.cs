using CmlLib.Core.VersionMetadata;

namespace GamerVII.Launcher.Models.Client;

internal class MinecraftVersion : IMinecraftVersion
{
    public string Version { get; set; } = null!;
    public string VersionType { get; set; }
    public MVersionMetadata? MVersion { get; set; }
}
