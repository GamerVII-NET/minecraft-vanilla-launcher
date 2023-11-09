using Newtonsoft.Json;

namespace GamerVII.Launcher.Models.Mods.Modrinth;

public class MHashes
{
    [JsonProperty("sha512")] public string Sha512 { get; set; }

    [JsonProperty("sha1")] public string Sha1 { get; set; }
}
