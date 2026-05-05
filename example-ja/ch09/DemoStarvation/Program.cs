using System.Diagnostics;

Console.WriteLine("第 9 章のデモ: スレッド プール枯渇");
Console.WriteLine("Note: この例は意図的に枯渇を作るため、完了まで数十秒かかることがあります。");

// 影響をわかりやすくするため、ThreadPool の最小スレッド数を意図的に下げる。
// これにより、突然押し寄せる作業量に対して初期の作業要員がかなり少なくなり、
// 枯渇の影響が強調される。
ThreadPool.SetMinThreads(1, 1);

// 500 件の並行 HTTP リクエストが一度に到着する状況をシミュレートする
int requestCount = 500;
var tasks = new Task[requestCount];
bool useProperAsyncVersion = false;

var sw = Stopwatch.StartNew();

for (int i = 0; i < requestCount; i++)
{
    int requestId = i;
    // 各リクエストは ThreadPool 上で動く作業項目である

    // 既定では sync-over-async を使い、間違ったアプローチによる遅延を示す。
    // 健全な async 版を見るには、useProperAsyncVersion を true に変更する。
    tasks[i] = useProperAsyncVersion
        ? Task.Run(() => ProcessRequestProperlyAsync(requestId))
        : Task.Run(() => ProcessRequestSyncOverAsync(requestId));
}

await Task.WhenAll(tasks);
sw.Stop();

Console.WriteLine($"\nすべての {requestCount} 件のリクエストを処理しました!");
Console.WriteLine($"合計経過時間: {sw.ElapsedMilliseconds} ms");

// --- 間違った例: 枯渇を引き起こす sync-over-async ---
void ProcessRequestSyncOverAsync(int id)
{
    // [致命的な誤り] バックグラウンド スレッド上で async メソッドを同期的に待つ。
    // これにより貴重な ThreadPool スレッドが約 1 秒拘束され、ほかのリクエストを処理できなくなる。
    // この例では、最小スレッド数を 1 に設定した状態で 500 個の作業項目を一度にキューイングするため、
    // ThreadPool はスレッドを少しずつ増やしながら対応することになり、全体のレイテンシが大きく伸びる。
    
    LogThreadCount(id, "開始");
    
    var result = SimulateDatabaseQueryAsync().Result; // <-- 問題の根本
    
    LogThreadCount(id, "終了");
}

// --- 正しい例: 最後まで async ---
async Task ProcessRequestProperlyAsync(int id)
{
    // [よい実践] await は I/O 待機中、ただちにスレッドを ThreadPool へ返す。
    // 500 件のリクエストが一度に到着しても、スレッドは待機状態でブロックされ続けないため、
    // 通常は少数のスレッドだけで大量の I/O 作業を進め続けられる。
    
    LogThreadCount(id, "開始");
    
    var result = await SimulateDatabaseQueryAsync(); // <-- スレッドを解放する
    
    LogThreadCount(id, "終了");
}

async Task<string> SimulateDatabaseQueryAsync()
{
    // 1 秒かかるネットワーク I/O またはデータベース クエリをシミュレートする
    await Task.Delay(1000);
    return "データ";
}

void LogThreadCount(int id, string state)
{
    ThreadPool.GetAvailableThreads(out int workerThreads, out _);
    ThreadPool.GetMaxThreads(out int maxWorkerThreads, out _);
    int activeThreads = maxWorkerThreads - workerThreads;
    
    Console.WriteLine($"[リクエスト {id:D3}] {state} - アクティブなプール スレッド: {activeThreads}");
}
