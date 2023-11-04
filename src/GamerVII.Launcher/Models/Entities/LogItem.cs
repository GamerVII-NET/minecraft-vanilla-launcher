using System;
using SQLite;

namespace GamerVII.Launcher.Models.Entities;

[Table("Logs")]
public class LogItem
{
    public string Date { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? StackTrace { get; set; }
}
