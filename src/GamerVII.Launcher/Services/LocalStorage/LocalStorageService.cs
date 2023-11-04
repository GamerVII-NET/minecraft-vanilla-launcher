using System.Threading.Tasks;
using GamerVII.Launcher.Models.Entities;
using Newtonsoft.Json;
using SQLite;

namespace GamerVII.Launcher.Services.LocalStorage;

public class LocalStorageService : ILocalStorageService
{
    private const string DatabasePath = "data.db";
    private readonly SQLiteAsyncConnection _database;

    public LocalStorageService()
    {
        _database = new SQLiteAsyncConnection(DatabasePath);

        InitializeTables();
    }

    private void InitializeTables()
    {
        _database.CreateTableAsync<StorageItem>().Wait();
        _database.CreateTableAsync<LogItem>().Wait();
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var serializedValue = JsonConvert.SerializeObject(value);
        var storageItem = new StorageItem
        {
            Key = key,
            TypeName = typeof(T).FullName,
            Value = serializedValue
        };
        await _database.InsertOrReplaceAsync(storageItem);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var storageItem = await _database.Table<StorageItem>()
            .Where(si => si.Key == key)
            .FirstOrDefaultAsync();

        if (storageItem != null)
        {
            return JsonConvert.DeserializeObject<T>(storageItem.Value);
        }

        return default(T);
    }

    public Task<int> SaveRecord<T>(T record)
    {
        return _database.InsertOrReplaceAsync(record);
    }

    [Table("StorageItems")]
    private class StorageItem
    {
        [PrimaryKey] public string Key { get; set; } = null!;
        public string? TypeName { get; set; }
        public string Value { get; set; } = null!;
    }
}
