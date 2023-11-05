using SQLite;

namespace GamerVII.Launcher.Models.Entities;

[Table("StorageItems")]
internal class StorageItem
{
    [PrimaryKey] public string Key { get; init; } = null!;
    public string? TypeName { get; set; }
    public string Value { get; init; } = null!;
}
