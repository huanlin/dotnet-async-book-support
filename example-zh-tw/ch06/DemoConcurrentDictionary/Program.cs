using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

Console.WriteLine("示範 ConcurrentDictionary 的 GetOrAdd 原子性操作");

var cache = new ConcurrentDictionary<string, string>();

// 兩個執行緒同時嘗試為同一個 key 載入資料
Task.WaitAll(
    Task.Run(() => cache.GetOrAdd("user:1", key => LoadDataFromDb(key))),
    Task.Run(() => cache.GetOrAdd("user:1", key => LoadDataFromDb(key)))
);

// 陷阱：即使最後只有一份資料會成功寫入字典，
// 但在競爭狀態下，LoadDataFromDb 很有可能會被執行兩次！
Console.WriteLine($"快取中的值: {cache["user:1"]}");

static string LoadDataFromDb(string key)
{
    Console.WriteLine($"[傳統寫法] 正在從資料庫為 {key} 載入資料...");
    // 模擬稍微耗時的讀取
    Task.Delay(100).Wait();
    return "Some Data";
}

Console.WriteLine("\n-------------------------------------------------------------");
Console.WriteLine("完美解法：搭配 Lazy<T> 保證只執行一次");

var lazyCache = new ConcurrentDictionary<string, Lazy<string>>();

Task.WaitAll(
    Task.Run(() => GetCachedData("user:2")),
    Task.Run(() => GetCachedData("user:2"))
);

Console.WriteLine($"快取中的值: {lazyCache["user:2"].Value}");

string GetCachedData(string key)
{
    var lazyResult = lazyCache.GetOrAdd(key,
        k => new Lazy<string>(() => LoadDataFromDbSafely(k)));

    return lazyResult.Value;
}

static string LoadDataFromDbSafely(string key)
{
    Console.WriteLine($"[Lazy 安全寫法] 正在從資料庫為 {key} 載入資料...");
    Task.Delay(100).Wait();
    return "Safe Data";
}
