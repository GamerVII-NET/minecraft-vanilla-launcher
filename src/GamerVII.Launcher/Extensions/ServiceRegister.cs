using Avalonia;
using GamerVII.Launcher.Services.Auth;
using GamerVII.Launcher.Services.Client;
using GamerVII.Launcher.Services.GameLaunch;
using GamerVII.Launcher.Services.LocalStorage;
using GamerVII.Launcher.Services.Logger;
using GamerVII.Launcher.Services.Mods;
using GamerVII.Launcher.Services.System;
using Splat;

namespace GamerVII.Launcher.Extensions;

public static class ServiceRegister
{

    public static AppBuilder RegisterServices(this AppBuilder builder)
    {
        RegisterServices();

        return builder;
    }

    public static void RegisterServices()
    {
        Locator.CurrentMutable.RegisterConstant(new LocalStorageService(), typeof(ILocalStorageService));
        Locator.CurrentMutable.RegisterConstant(new DatabaseLoggerService(), typeof(ILoggerService));
        Locator.CurrentMutable.RegisterConstant(new SystemService(), typeof(ISystemService));
        Locator.CurrentMutable.RegisterConstant(new LocalGameClientService(), typeof(IGameClientService));
        Locator.CurrentMutable.RegisterConstant(new GameLaunchService(), typeof(IGameLaunchService));
        Locator.CurrentMutable.RegisterConstant(new EmptyAuthService(), typeof(IAuthService));
        Locator.CurrentMutable.RegisterConstant(new ModrinthModsService(), typeof(IModsService));
    }
}
