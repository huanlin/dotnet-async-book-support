using System.Diagnostics;

Console.WriteLine("示範第 9 章：執行緒集區飢餓 (Thread Pool Starvation)");
Console.WriteLine("注意：此範例將會刻意製造飢餓現象，導致程式耗時數十秒才能完成。");

// 為了讓效果更明顯，刻意把 ThreadPool 的最小執行緒數降到很低，
// 讓一開始可用的人力遠低於瞬間湧入的工作量，藉此放大飢餓現象。
ThreadPool.SetMinThreads(1, 1);

// 模擬瞬間湧入 500 個併發的 HTTP Request
int requestCount = 500;
var tasks = new Task[requestCount];
bool useProperAsyncVersion = false;

var sw = Stopwatch.StartNew();

for (int i = 0; i < requestCount; i++)
{
    int requestId = i;
    // 每個 Request 都是一個跑在 ThreadPool 上的工作

    // 預設使用 sync-over-async 來展示錯誤寫法造成的延遲現象。
    // 若要改看健康的非同步版本，請將 useProperAsyncVersion 改成 true。
    tasks[i] = useProperAsyncVersion
        ? Task.Run(() => ProcessRequestProperlyAsync(requestId))
        : Task.Run(() => ProcessRequestSyncOverAsync(requestId));
}

await Task.WhenAll(tasks);
sw.Stop();

Console.WriteLine($"\n全部 {requestCount} 個請求處理完畢！");
Console.WriteLine($"總耗時: {sw.ElapsedMilliseconds} ms");

// --- 錯誤示範：導致飢餓的 Sync-over-Async ---
void ProcessRequestSyncOverAsync(int id)
{
    // [致命錯誤] 在背景執行緒中「同步等待」一個非同步方法。
    // 這會把珍貴的 ThreadPool 執行緒卡住約 1 秒，無法去處理其他請求。
    // 在這個範例裡，我們同時丟出 500 個工作，卻把最小執行緒數壓到 1，
    // 因此 ThreadPool 只能一邊救火、一邊慢慢補執行緒，整體延遲就會被大幅拉長。
    
    LogThreadCount(id, "開始");
    
    var result = SimulateDatabaseQueryAsync().Result; // <-- 災難的源頭
    
    LogThreadCount(id, "結束");
}

// --- 正確示範：一路非同步到底 ---
async Task ProcessRequestProperlyAsync(int id)
{
    // [良好實踐] await 會在等待 I/O 期間立刻釋放執行緒回 ThreadPool。
    // 即使瞬間湧入 500 個請求，執行緒也不必一直被卡在等待狀態，
    // 因此系統通常只需要少量執行緒，就能把大量 I/O 工作順利推進。
    
    LogThreadCount(id, "開始");
    
    var result = await SimulateDatabaseQueryAsync(); // <-- 釋放執行緒
    
    LogThreadCount(id, "結束");
}

async Task<string> SimulateDatabaseQueryAsync()
{
    // 模擬網路 I/O 或資料庫查詢，耗時 1 秒
    await Task.Delay(1000);
    return "Data";
}

void LogThreadCount(int id, string state)
{
    ThreadPool.GetAvailableThreads(out int workerThreads, out _);
    ThreadPool.GetMaxThreads(out int maxWorkerThreads, out _);
    int activeThreads = maxWorkerThreads - workerThreads;
    
    Console.WriteLine($"[請求 {id:D3}] {state} - 目前活動的集區執行緒數: {activeThreads}");
}
