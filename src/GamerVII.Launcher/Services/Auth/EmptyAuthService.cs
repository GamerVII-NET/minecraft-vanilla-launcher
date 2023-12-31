﻿using System;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.LocalStorage;
using Splat;

namespace GamerVII.Launcher.Services.Auth;

public class EmptyAuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;

    public EmptyAuthService(ILocalStorageService? localStorage = null)
    {
        _localStorage = localStorage
                        ?? Locator.Current.GetService<ILocalStorageService>()
                        ?? throw new Exception(nameof(ILocalStorageService) + " not registered");
    }

    public async Task<IUser> GetAuthorizedUser()
    {
        var user = await _localStorage.GetAsync<User>("User");

        return user ?? new User { Login = string.Empty, Password = string.Empty};
    }

    public Task<IUser> OnLogin(string login, string password)
    {
        var user = new User
        {
            Login = login,
            Password = password,
            AccessToken = string.Empty,
            IsLogin = true
        };

        _localStorage.SetAsync("User", user);

        return Task.FromResult((IUser)user);
    }

    public Task OnLogout()
    {
        var user = new User
        {
            Login = string.Empty,
            Password = string.Empty,
            AccessToken = string.Empty,
            IsLogin = false
        };

        return _localStorage.SetAsync("User", user);
    }
}
