using System;
using System.Collections.Generic;

namespace GamerVII.Launcher.Models.Client;

public class ClientMod : IMod
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public int Downloads { get; set; }
    public int Follows { get; set; }
    public string IconUrl { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset DateModified { get; set; }
    public string LatestVersion { get; set; }
    public string Author { get; set; }
    public List<string> Gallery { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is IMod mod)
        {
            return this.Slug == mod.Slug;
        }

        return false;
    }
}
