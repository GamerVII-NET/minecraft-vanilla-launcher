using CmlLib.Core.VersionMetadata;

namespace GamerVII.Launcher.Models.Client;

public class MinecraftVersion : IMinecraftVersion
{
    public string Version { get; set; }
    public MVersionMetadata MVersion { get; set; }
}
