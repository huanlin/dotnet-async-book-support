using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

Console.WriteLine("示範 ConcurrentDictionary 的 GetOrAdd 原子性操作");

var cache = new ConcurrentDictionary<string, string>();

// 兩個執行緒同時嘗試為同一個 key 新增值
Task.WaitAll(
    Task.Run(() => cache.GetOrAdd("user:1", key => LoadDataFromDb(key))),
    Task.Run(() => cache.GetOrAdd("user:1", key => LoadDataFromDb(key)))
);

// 即使兩個執行緒同時呼叫，LoadDataFromDb 也有極高機率只會被執行一次，
// 且後續取值保證安全一致。
Console.WriteLine($"快取中的值: {cache["user:1"]}");

static string LoadDataFromDb(string key)
{
    Console.WriteLine($"正在從資料庫為 {key} 載入資料...");
    // 模擬稍微耗時的讀取
    Task.Delay(100).Wait();
    return "Some Data";
}
