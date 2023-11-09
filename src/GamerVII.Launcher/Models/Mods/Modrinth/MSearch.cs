﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace GamerVII.Launcher.Models.Mods.Modrinth;

public class MSearch
{
    [JsonProperty("hits")]
    public List<MHit> Hits { get; set; }

    [JsonProperty("offset")]
    public int Offset { get; set; }

    [JsonProperty("limit")]
    public int Limit { get; set; }

    [JsonProperty("total_hits")]
    public int TotalHits { get; set; }
}