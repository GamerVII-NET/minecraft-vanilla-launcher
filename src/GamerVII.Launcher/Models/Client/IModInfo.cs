namespace GamerVII.Launcher.Models.Client;

public interface IModInfo
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string ShortDescription { get; set; }
    public string FullDescription { get; set; }
}
