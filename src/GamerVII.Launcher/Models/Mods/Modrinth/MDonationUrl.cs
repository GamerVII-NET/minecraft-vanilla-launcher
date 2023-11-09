using Newtonsoft.Json;

namespace GamerVII.Launcher.Models.Mods.Modrinth;

public class MDonationUrl
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("platform")] public string Platform { get; set; }

    [JsonProperty("url")] public string Url { get; set; }
}
