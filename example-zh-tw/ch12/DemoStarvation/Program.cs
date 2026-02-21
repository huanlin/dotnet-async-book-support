using System.Diagnostics;

Console.WriteLine("示範第 12 章：執行緒集區飢餓 (Thread Pool Starvation)");
Console.WriteLine("注意：此範例將會刻意製造飢餓現象，導致程式耗時數十秒才能完成。");

// 為了讓效果明顯，我們先把 ThreadPool 的最小執行緒數限制在常見機器的核心數 (例如: 8)
ThreadPool.SetMinThreads(8, 8);

// 模擬瞬間湧入 100 個併發的 HTTP Request
int requestCount = 100;
var tasks = new Task[requestCount];

var sw = Stopwatch.StartNew();

for (int i = 0; i < requestCount; i++)
{
    int requestId = i;
    // 每個 Request 都是一個跑在 ThreadPool 上的工作
    
    // 底下這行是呼叫 sync-over-async 來展示錯誤寫法造成的延遲現象
    tasks[i] = Task.Run(() => ProcessRequestSyncOverAsync(requestId));

    // 底下這行是健康的非同步 API。把上一行變成註解，並將底下這行去掉註解，以觀察效能差異。
    // tasks[i] = Task.Run(() => ProcessRequestProperlyAsync(requestId));
}

await Task.WhenAll(tasks);
sw.Stop();

Console.WriteLine($"\n全部 {requestCount} 個請求處理完畢！");
Console.WriteLine($"總耗時: {sw.ElapsedMilliseconds} ms");

// --- 錯誤示範：導致飢餓的 Sync-over-Async ---
void ProcessRequestSyncOverAsync(int id)
{
    // [致命錯誤] 在背景執行緒中「同步等待」一個非同步方法
    // 這會把這條珍貴的 ThreadPool 執行緒卡住 (block)長達 1 秒！
    // 由於我們只預先開了 8 條執行緒，瞬間就會全部被卡死。
    // 剩下的 92 個請求，必須等待 ThreadPool 以每秒約 1~2 條的龜速緩慢「注入 (inject)」新執行緒。
    
    LogThreadCount(id, "開始");
    
    var result = SimulateDatabaseQueryAsync().Result; // <-- 災難的源頭
    
    LogThreadCount(id, "結束");
}

// --- 正確示範：一路非同步到底 ---
async Task ProcessRequestProperlyAsync(int id)
{
    // [良好實踐] await 會立刻釋放 (Yield) 執行緒回 ThreadPool 
    // 這 8 條執行緒可以瞬間接手所有的 100 個請求並發送給資料庫。
    // 即便瞬間湧入 100 個連線，系統也只會用到少量的執行緒，1 秒出頭就能全數處理完畢！
    
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
