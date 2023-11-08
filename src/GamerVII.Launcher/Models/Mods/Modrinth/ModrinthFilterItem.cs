namespace GamerVII.Launcher.Models.Mods.Modrinth;

public class ModrinthFilterItem : IFilterItem
{
    public string Key { get; set; }
    public string Value { get; set; }

    public ModrinthFilterItem(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public override string ToString()
    {
        return @$"[""{Key}:{Value}""]";
    }
}
