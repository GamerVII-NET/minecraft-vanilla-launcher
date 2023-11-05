using CmlLib.Core.VersionMetadata;

namespace GamerVII.Launcher.Models.Client;

internal class MinecraftVersion : IMinecraftVersion
{
    public string Version { get; set; } = null!;
    public MVersionMetadata? MVersion { get; set; }
}
