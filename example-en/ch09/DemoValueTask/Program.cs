using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

Console.WriteLine("Demonstrating ValueTask<T> in a caching scenario");

var cache = new MemoryCache(new MemoryCacheOptions());
var service = new MyService(cache);

Console.WriteLine("First query (miss, runs asynchronously):");
string result1 = await service.GetValueAsync("key1");
Console.WriteLine($"Retrieved: {result1}");

Console.WriteLine("\nSecond query (hit, completes synchronously):");
string result2 = await service.GetValueAsync("key1");
Console.WriteLine($"Retrieved: {result2}");

public class MyService
{
    private readonly IMemoryCache _cache;

    public MyService(IMemoryCache cache)
    {
        _cache = cache;
    }

    // Use ValueTask<T> to optimize cache lookups
    public async ValueTask<string> GetValueAsync(string key)
    {
        // Case 1: cache hit (completes synchronously)
        // Return the result directly, with no Task allocation at all
        if (_cache.TryGetValue(key, out string? cachedValue) && cachedValue != null)
        {
            Console.WriteLine("   -> Cache hit");
            return cachedValue;
        }

        // Case 2: cache miss (runs asynchronously)
        // Only here do we actually await a Task and incur an allocation
        string valueFromDb = await GetValueFromDatabaseAsync(key);
        _cache.Set(key, valueFromDb, TimeSpan.FromMinutes(5));
        return valueFromDb;
    }

    private async Task<string> GetValueFromDatabaseAsync(string key)
    {
        Console.WriteLine("   -> Querying the database...");
        await Task.Delay(1000); // Simulate I/O latency
        return $"ValueFor_{key}";
    }
}
