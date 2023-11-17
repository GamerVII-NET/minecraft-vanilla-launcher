using System.Collections.Generic;

namespace GamerVII.Launcher.Models.Client;

public class ModVersion : IModVersion
{
    public string Name { get; set; }
    public IEnumerable<string> Files { get; set; }
}
