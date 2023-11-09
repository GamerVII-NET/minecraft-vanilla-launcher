using GamerVII.Launcher.Models.Enums;

namespace GamerVII.Launcher.Models.Client;

public interface IGameLoader
{
    public string Name { get; set; }
    public ModLoaderType LoaderType { get; set; }
}
