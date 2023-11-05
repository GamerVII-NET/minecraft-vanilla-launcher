namespace GamerVII.Launcher.Models.Client;

internal class StartupOptions : IStartupOptions
{
    public int MinimumRamMb { get; set; }
    public int MaximumRamMb { get; set; }
    public bool FullScreen { get; set; }
    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }
    public string? ServerIp { get; set; }
    public int ServerPort { get; set; }
}
