using System.IO;
using CmlLib.Core;

namespace GamerVII.Launcher.Models.Client;

internal class CustomMinecraftPath : MinecraftPath
{
    public CustomMinecraftPath()
    {
    }

    public CustomMinecraftPath(string basePath) : base(basePath)
    {
        BasePath = NormalizePath(basePath);

        Library = NormalizePath(Path.Combine(BasePath, "libraries"));
        Versions = NormalizePath(Path.Combine(BasePath, "versions"));
        Resource = NormalizePath(Path.Combine(BasePath, "resources"));

        Runtime = NormalizePath(Path.Combine(BasePath, "runtime"));
        Assets = NormalizePath(Path.Combine(BasePath, "assets"));

        CreateDirs();
    }

    public CustomMinecraftPath(string basePath, string basePathForAssets) : base(basePath, basePathForAssets)
    {
    }




}
