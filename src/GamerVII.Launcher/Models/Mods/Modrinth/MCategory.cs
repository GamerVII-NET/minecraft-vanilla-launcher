using GamerVII.Launcher.Models.Client;
using Newtonsoft.Json;

namespace GamerVII.Launcher.Models.Mods.Modrinth;

public class MCategory : IModCategory
{
    [JsonProperty("icon")] public string Icon { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("project_type")] public string Type { get; set; }

    [JsonProperty("header")] public string Header { get; set; }
}
