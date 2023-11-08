using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GamerVII.Launcher.Models.Client;

public interface IMod
{
    public string Title { get; set; }

    public string Description { get; set; }

    public int Downloads { get; set; }

    public int Follows { get; set; }

    public string IconUrl { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public DateTimeOffset DateModified { get; set; }

    public string LatestVersion { get; set; }

    public string Author { get; set; }

    public List<string> Gallery { get; set; }
}
