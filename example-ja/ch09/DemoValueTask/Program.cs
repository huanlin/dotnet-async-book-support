using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

Console.WriteLine("キャッシュ シナリオで ValueTask<T> を使うデモ");

var cache = new MemoryCache(new MemoryCacheOptions());
var service = new MyService(cache);

Console.WriteLine("1 回目の問い合わせ (ミス、非同期で実行):");
string result1 = await service.GetValueAsync("key1");
Console.WriteLine($"取得結果: {result1}");

Console.WriteLine("\n2 回目の問い合わせ (ヒット、同期的に完了):");
string result2 = await service.GetValueAsync("key1");
Console.WriteLine($"取得結果: {result2}");

public class MyService
{
    private readonly IMemoryCache _cache;

    public MyService(IMemoryCache cache)
    {
        _cache = cache;
    }

    // ValueTask<T> を使ってキャッシュ検索を最適化する
    public async ValueTask<string> GetValueAsync(string key)
    {
        // ケース 1: キャッシュ ヒット (同期的に完了する)
        // Task をまったく割り当てず、結果を直接返す
        if (_cache.TryGetValue(key, out string? cachedValue) && cachedValue != null)
        {
            Console.WriteLine("   -> キャッシュ ヒット");
            return cachedValue;
        }

        // ケース 2: キャッシュ ミス (非同期に実行される)
        // 実際に Task を await して割り当てが発生するのはここだけである
        string valueFromDb = await GetValueFromDatabaseAsync(key);
        _cache.Set(key, valueFromDb, TimeSpan.FromMinutes(5));
        return valueFromDb;
    }

    private async Task<string> GetValueFromDatabaseAsync(string key)
    {
        Console.WriteLine("   -> データベースに問い合わせています...");
        await Task.Delay(1000); // I/O レイテンシをシミュレートする
        return $"{key} の値";
    }
}
