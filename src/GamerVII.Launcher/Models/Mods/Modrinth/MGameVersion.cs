using System;
using GamerVII.Launcher.Models.Client;
using Newtonsoft.Json;

namespace GamerVII.Launcher.Models.Mods.Modrinth;

public class MGameVersion : IMinecraftVersion
{
    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("version_type")]
    public string VersionType { get; set; }

    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("major")]
    public bool Major { get; set; }
}
