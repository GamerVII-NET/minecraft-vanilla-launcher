using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Mods;
using GamerVII.Launcher.Models.Mods.Modrinth;
using Newtonsoft.Json;

namespace GamerVII.Launcher.Services.Mods;

public class ModrinthModsService : IModsService
{
    private readonly HttpClient _httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(30),
        BaseAddress = new Uri("https://api.modrinth.com")
    };

    public ModrinthModsService()
    {
        // _httpClient.DefaultRequestHeaders.Add("User-Agent", "GamerVII-NET/minecraft-vanilla-launcher/1.1.0 (launcher.recloud.tech)");
    }

    public async Task<IEnumerable<IMod>> GetModsListAsync(IFilter<IFilterItem> filter)
    {
        filter.AddIfNotExists(new ModrinthFilterItem("project_type", "mod"));

        var uri = filter.GetParametersString("/v2/search");

        var request = await _httpClient.GetAsync(uri);

        if (!request.IsSuccessStatusCode)
            return Enumerable.Empty<IMod>();

        var data = await request.Content.ReadAsStringAsync();

        var rootData = JsonConvert.DeserializeObject<Search>(data);

        return rootData?.Hits.Select(c => new ClientMod
        {
            Author = c.Author,
            Description = c.Description,
            Downloads = c.Downloads,
            Follows = c.Follows,
            Gallery = c.Gallery,
            Title = c.Title,
            DateCreated = c.DateCreated,
            DateModified = c.DateModified,
            IconUrl = c.IconUrl,
        }) ?? Enumerable.Empty<IMod>();
    }
}
