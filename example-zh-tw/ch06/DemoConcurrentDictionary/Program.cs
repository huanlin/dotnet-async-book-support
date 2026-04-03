using System.Collections.Concurrent;

DemoGetOrAddTrap();

Console.WriteLine("\n-------------------------------------------------------------");

DemoGetOrAddWithLazy();


static void DemoGetOrAddTrap()
{
    Console.WriteLine("示範 ConcurrentDictionary 的 GetOrAdd 與 valueFactory 行為");

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
        Console.WriteLine($"正在從資料庫為 {key} 載入資料...");
        // 這裡刻意模擬「同步」工作，所以用 Thread.Sleep；
        // 若寫成 Task.Delay(...).Wait() 反而會變成不建議的 sync-over-async。
        Thread.Sleep(100);
        return "Some Data";
    }
}

static void DemoGetOrAddWithLazy()
{
    Console.WriteLine("常見解法：搭配 Lazy<T> 延後真正的初始化");

    var cache = new ConcurrentDictionary<string, Lazy<string>>();

    Task.WaitAll(
        Task.Run(() => GetCachedData("user:1")),
        Task.Run(() => GetCachedData("user:1"))
    );

    Console.WriteLine($"快取中的值: {cache["user:1"].Value}");

    string GetCachedData(string key)
    {
        // 工廠函式只建立 Lazy<T> 包裝物件。
        // 真正的初始化會延後到存活下來的 Lazy<T>.Value 被讀取時才發生。
        var lazyResult = cache.GetOrAdd(key,
            k => new Lazy<string>(
                () => LoadDataFromDb(k),
                LazyThreadSafetyMode.ExecutionAndPublication));

        // 取得 Value 時才會真正執行 LoadDataFromDb，
        // 並由存活下來的 Lazy<T> 負責完成一次初始化
        return lazyResult.Value;
    }

    static string LoadDataFromDb(string key)
    {
        Console.WriteLine($"正在從資料庫為 {key} 載入資料...");
        // 同上：這裡不是 async 流程，若真要模擬非同步延遲，應改寫成 await Task.Delay(...)。
        Thread.Sleep(100);
        return "Safe Data";
    }
}
