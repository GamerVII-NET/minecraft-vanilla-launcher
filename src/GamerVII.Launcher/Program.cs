using Avalonia;
using Avalonia.ReactiveUI;
using GamerVII.Launcher.Extensions;
using System;
using System.Reactive;
using GamerVII.Launcher.Services.Logger;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher
{
    internal class Program
    {
        private static ILoggerService _loggerService;

        [STAThread]
        public static void Main(string[] args)
        {

            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(GlobalExceptionHandler);

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        private static void GlobalExceptionHandler(Exception exception)
        {
            _loggerService.Log(exception.Message, exception);
        }


        public static AppBuilder BuildAvaloniaApp()
        {

            var app = AppBuilder.Configure<App>()
                                .UsePlatformDetect()
                                .WithInterFont()
                                .LogToTrace()
                                .RegisterServices()
                                .UseReactiveUI();

            _loggerService = Locator.Current.GetService<ILoggerService>() ?? throw new Exception($"{nameof(ILoggerService)} not registered");

            return app;

        }
    }
}
