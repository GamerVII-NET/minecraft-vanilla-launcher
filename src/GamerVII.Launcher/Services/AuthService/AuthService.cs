using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.LocalStorage;
using Microsoft.IdentityModel.Tokens;
using Splat;

namespace GamerVII.Launcher.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;

    public AuthService(ILocalStorageService localStorage = null)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost")
        };

        _localStorage = localStorage ?? Locator.Current.GetService<ILocalStorageService>()!;
    }

    public async Task<IUser> OnLogin(string login, string password)
    {
        IUser user = new User
        {
            Login = login,
            Password = password
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress!.AbsoluteUri}/auth.php");

        var content = new MultipartFormDataContent();
        content.Add(new StringContent(user.Login), "login");
        content.Add(new StringContent(user.Password), "password");

        request.Content = content;
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode != System.Net.HttpStatusCode.OK) return user;

        var result = await response.Content.ReadAsStringAsync();
        user.IsLogin = true;
        user.AccessToken = result;
        var refreshToken = response.Headers.FirstOrDefault(c => c.Key == "Refresh-Token").Value.FirstOrDefault() ?? string.Empty;

        await _localStorage.SetAsync("RefreshToken", refreshToken);
        await _localStorage.SetAsync("User", user);

        return user;

    }

    public async Task OnLogout()
    {
        await _localStorage.SetAsync("RefreshToken", string.Empty);
        await _localStorage.SetAsync("User", string.Empty);
    }

    public async Task<IUser?> GetAuthorizedUser()
    {
        return await _localStorage.GetAsync<User>("User");
    }
}
