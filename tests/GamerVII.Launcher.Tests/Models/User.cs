using GamerVII.Launcher.Models.Users;

namespace GamerVII.Launcher.Tests.Models;

public class User : IUser
{
    public string Login { get; set; } = null!;
    public string? Password { get; set; }
    public bool IsLogin { get; set; }
    public string? AccessToken { get; set; }
}
