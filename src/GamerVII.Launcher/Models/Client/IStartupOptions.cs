namespace GamerVII.Launcher.Models.Client;

public interface IStartupOptions
{
    int MinimumRamMb { get; set; }
    int MaximumRamMb { get; set; }
    bool FullScreen { get; set; }
    int ScreenWidth { get; set; }
    int ScreenHeight { get; set; }
    string? ServerIp { get; set; }
    int ServerPort { get; set; }
}
