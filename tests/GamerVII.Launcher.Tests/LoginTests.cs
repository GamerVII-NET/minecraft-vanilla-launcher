using System.Diagnostics;
using GamerVII.Launcher.Extensions;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.Auth;
using GamerVII.Launcher.Services.Client;
using GamerVII.Launcher.Services.GameLaunch;
using GamerVII.Launcher.Services.LocalStorage;
using GamerVII.Launcher.Services.Logger;
using GamerVII.Launcher.ViewModels;
using Splat;

namespace GamerVII.Launcher.Tests;

public class Tests
{
    private AuthPageViewModel _authPageViewModel;
    private MainWindowViewModel _mainViewModel;
    private LocalStorageService _storageService;
    private bool _isAuthorized;


    [SetUp]
    public void Setup()
    {

        ServiceRegister.RegisterServices();

        _authPageViewModel = new AuthPageViewModel
        {
            Login = "GamerVII",
            Password = "password"
        };

        _mainViewModel = new MainWindowViewModel();

        _authPageViewModel.Authorized += async user =>
        {
            _isAuthorized = user.IsLogin;

            await _storageService.SetAsync("User", user);
        };

        _storageService = new LocalStorageService();

    }

    [Test]
    public void CanExecuteAuthTest()
    {
        Assert.That(_authPageViewModel.OnLoginCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void AuthTest()
    {
        _authPageViewModel.OnLoginCommand.Execute(null);

        Assert.That(_isAuthorized, Is.True);
    }

    [Test]
    public async Task UserIsSavedTest()
    {
        var user = await _storageService.GetAsync<User>("User");

        Assert.That(user, Is.Not.Null);
    }

    [Test]
    public async Task SavedUserCanLogInTest()
    {
        var user = await _storageService.GetAsync<User>("User");

        if (user != null)
        {
            _authPageViewModel = new AuthPageViewModel
            {
                Login = user.Login,
                Password = user.Password
            };

            Assert.Multiple(() =>
            {
                Assert.That(_authPageViewModel.OnLoginCommand.CanExecute(null), Is.True);

                _authPageViewModel.OnLoginCommand.Execute(null);

                Assert.That(_isAuthorized, Is.True);
            });

        }


        Assert.That(user, Is.Not.Null);
    }
}
