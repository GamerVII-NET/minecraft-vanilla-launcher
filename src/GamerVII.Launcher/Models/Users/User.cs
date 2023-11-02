﻿
namespace GamerVII.Launcher.Models.Users;

public class User : IUser
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsLogin { get; set; }
    public string? AccessToken { get; set; }
}
