using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GamerVII.Launcher.Services.Logger;
using Splat;

namespace GamerVII.Launcher.Services.LocalStorage;

public class LocalStorageService : ILocalStorageService
{
    private readonly string _storagePath = "config.data";

    private Dictionary<string, string>? _storage;
    private readonly ILoggerService _loggerService;

    public LocalStorageService(ILoggerService? loggerService = null)
    {
        _loggerService = loggerService
                         ?? Locator.Current.GetService<ILoggerService>()
                         ?? throw new Exception($"{nameof(ILoggerService)} not registered");
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        _storage ??= await ReadDataFromFile();

        return _storage.TryGetValue(key, out string? jsonValue) ? JsonConvert.DeserializeObject<T>(jsonValue) : default;
    }

    public async Task SetAsync<T>(string key, T value)
    {
        _storage ??= await ReadDataFromFile();

        _storage[key] = JsonConvert.SerializeObject(value);

        await SaveDataToFile(_storage);
    }

    private async Task SaveDataToFile(Dictionary<string, string> storage)
    {
        await using var fileStream =
            new FileStream(_storagePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        await using var streamWriter = new StreamWriter(fileStream);
        var json = JsonConvert.SerializeObject(storage);
        await streamWriter.WriteAsync(json);
    }

    private async Task<Dictionary<string, string>> ReadDataFromFile()
    {
        if (!File.Exists(_storagePath)) return new Dictionary<string, string>();

        await using var fileStream = new FileStream(_storagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var streamReader = new StreamReader(fileStream);
        var json = await streamReader.ReadToEndAsync();

        var data = new Dictionary<string, string>();

        try
        {
            data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
        catch (Exception ex)
        {
            _loggerService.Log(ex.Message, ex);
        }

        return data;
    }
}
