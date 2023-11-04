using System;
using System.IO;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Entities;
using GamerVII.Launcher.Services.LocalStorage;
using Splat;

namespace GamerVII.Launcher.Services.Logger;

public class DatabaseLoggerService : ILoggerService
{
    private readonly ILocalStorageService _storageService;

    public DatabaseLoggerService(ILocalStorageService? storageService = null)
    {
        _storageService = storageService
                          ?? Locator.Current.GetService<ILocalStorageService>()
                          ?? throw new Exception($"{nameof(ILocalStorageService)} not registered!");
    }

    public async Task Log(string message)
    {
        Console.WriteLine(message);

        var logItem = new LogItem
        {
            Message = message,
            Date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss / zz"),
        };

        await _storageService.SaveRecord(logItem);
    }

    public async Task Log(string message, Exception exception)
    {

        Console.WriteLine(message);

        var logItem = new LogItem
        {
            Message = message,
            Date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss / zz"),
            StackTrace = exception.StackTrace
        };

        await _storageService.SaveRecord(logItem);
    }

    public void Dispose()
    {

    }
}
