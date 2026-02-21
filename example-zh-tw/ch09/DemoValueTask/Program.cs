using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

Console.WriteLine("示範 ValueTask<T> 用於快取情境");

var cache = new MemoryCache(new MemoryCacheOptions());
var service = new MyService(cache);

Console.WriteLine("第一次查詢 (未命中，非同步執行):");
string result1 = await service.GetValueAsync("key1");
Console.WriteLine($"取得: {result1}");

Console.WriteLine("\n第二次查詢 (命中，同步完成):");
string result2 = await service.GetValueAsync("key1");
Console.WriteLine($"取得: {result2}");

public class MyService
{
    private readonly IMemoryCache _cache;

    public MyService(IMemoryCache cache)
    {
        _cache = cache;
    }

    // 使用 ValueTask<T> 來優化快取查詢
    public async ValueTask<string> GetValueAsync(string key)
    {
        // 情況 1: 快取命中 (同步完成)
        // 直接回傳結果，沒有任何 Task 配置
        if (_cache.TryGetValue(key, out string? cachedValue) && cachedValue != null)
        {
            Console.WriteLine("   -> 快取命中");
            return cachedValue;
        }

        // 情況 2: 快取未命中 (非同步執行)
        // 這裡才會實際 await 一個 Task，並產生配置
        string valueFromDb = await GetValueFromDatabaseAsync(key);
        _cache.Set(key, valueFromDb, TimeSpan.FromMinutes(5));
        return valueFromDb;
    }

    private async Task<string> GetValueFromDatabaseAsync(string key)
    {
        Console.WriteLine("   -> 從資料庫查詢...");
        await Task.Delay(1000); // 模擬 I/O 延遲
        return $"ValueFor_{key}";
    }
}
