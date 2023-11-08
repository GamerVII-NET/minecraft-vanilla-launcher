using Newtonsoft.Json;

namespace GamerVII.Launcher.Models.Mods.Modrinth;

public class License
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("url")] public string Url { get; set; }
}
