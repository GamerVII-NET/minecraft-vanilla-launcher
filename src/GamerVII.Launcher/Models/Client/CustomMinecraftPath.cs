using System.IO;
using CmlLib.Core;

namespace GamerVII.Launcher.Models.Client;

internal class CustomMinecraftPath : MinecraftPath
{
    public CustomMinecraftPath(string basePath)
    {
        BasePath = NormalizePath(basePath);

        Library = NormalizePath(Path.Combine(BasePath, "libraries"));
        Versions = NormalizePath(Path.Combine(BasePath, "clients"));
        Resource = NormalizePath(Path.Combine(BasePath, "resources"));

        Runtime = NormalizePath(Path.Combine(BasePath, "java"));
        Assets = NormalizePath(Path.Combine(BasePath, "assets"));

        CreateDirs();
    }

    public override string GetVersionJarPath(string id) => NormalizePath($"{Versions}/{id}/client.jar");

    public override string GetVersionJsonPath(string id) => NormalizePath($"{Versions}/{id}/client.json");

    public override string GetAssetObjectPath(string assetId) => NormalizePath($"{Assets}/files");

}
