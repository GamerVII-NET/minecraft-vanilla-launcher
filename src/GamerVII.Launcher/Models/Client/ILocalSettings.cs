namespace GamerVII.Launcher.Models.Client;

public interface ILocalSettings
{
    int MemorySize { get; set; }
    int WindowWidth { get; set; }
    int WindowHeight { get; set; }
    bool IsFullScreen { get; set; }
}
