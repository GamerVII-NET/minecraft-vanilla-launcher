namespace GamerVII.Launcher.Models.Client;

public class LocalSettings : ILocalSettings
{
    public int MemorySize { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public bool IsFullScreen { get; set; }
}
