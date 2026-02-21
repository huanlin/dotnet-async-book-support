using System.Diagnostics;

Console.WriteLine("示範第 11 章：非同步 API 設計準則");

Console.WriteLine("\n--- 展示一：ValueTask 與快取 ---");
var api = new GoodApiDesignService();
var sw = Stopwatch.StartNew();

// 第一次呼叫 (會觸發真正的 Task，等待 1 秒)
Console.WriteLine(await api.GetDataAsync(1));
Console.WriteLine($"第一次呼叫耗時: {sw.ElapsedMilliseconds} ms");

// 第二次呼叫 (快取命中，使用 ValueTask 同步返回，不配置 Task)
sw.Restart();
Console.WriteLine(await api.GetDataAsync(1));
Console.WriteLine($"第二次呼叫耗時 (從快取): {sw.ElapsedMilliseconds} ms");


Console.WriteLine("\n--- 展示二：參數驗證的拋出時機 ---");
try
{
    // 因為參數錯誤，會立即被同步拋出，不會回傳 Faulted Task
    _ = api.DownloadFileAsync(string.Empty);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"成功捕捉同步 ArgumentException: {ex.Message}");
}


Console.WriteLine("\n--- 展示三：IAsyncDisposable 的資源釋放 ---");

// 使用 await using 優雅地確保底層資源非同步清理
await using (var wrapper = new NetworkStreamWrapper())
{
    await wrapper.SendAsync("Hello!");
} // 在這裡會自動非同步呼叫 wrapper.DisposeAsync()

Console.WriteLine("展示完畢。");


/// <summary>
/// 示範良好 API 設計的服務類別
/// </summary>
public class GoodApiDesignService
{
    private static readonly HttpClient s_httpClient = new HttpClient();
    private readonly Dictionary<int, string> _cache = new();

    // 1. 使用 ValueTask 結合快取避免 Task 的 GC 配置壓力
    // 2. 遵守 Async 結尾命名慣例
    // 3. 正確傳遞 CancellationToken (預設值為 default)
    public ValueTask<string> GetDataAsync(int id, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(id, out var data))
        {
            Console.WriteLine("[Service] 命中快取，直接同步回傳");
            return new ValueTask<string>(data);
        }

        // 呼叫輔助方法回傳 ValueTask
        return new ValueTask<string>(FetchAndCacheDataAsync(id, cancellationToken));
    }

    private async Task<string> FetchAndCacheDataAsync(int id, CancellationToken ct)
    {
        Console.WriteLine("[Service] 未命中快取，發起 I/O 請求...");
        // 4. 身為函式庫作者，謹記使用 ConfigureAwait(false) 避免捕捉環境
        await Task.Delay(1000, ct).ConfigureAwait(false);
        var result = $"Data for {id}";
        _cache[id] = result;
        return result;
    }

    // 5. 使用區域函數 (Local Function) 來保證參數檢查的例外能立即拋出
    public Task<string> DownloadFileAsync(string url, CancellationToken cancellationToken = default)
    {
        // 參數驗證 (同步發生)
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url 不可為空", nameof(url));

        return CoreDownloadAsync();

        async Task<string> CoreDownloadAsync()
        {
            return await s_httpClient.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        }
    }
}

/// <summary>
/// 示範 IAsyncDisposable 的實作
/// </summary>
public class NetworkStreamWrapper : IAsyncDisposable
{
    public NetworkStreamWrapper()
    {
        Console.WriteLine("[Wrapper] 建立網路連線");
    }

    public async Task SendAsync(string message)
    {
        Console.WriteLine($"[Wrapper] 發送訊息: {message}");
        await Task.Delay(100).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("[Wrapper] 準備釋放資源... (非同步清理開始)");
        
        // 模擬非同步發送「關閉連線」告別封包給伺服器
        await Task.Delay(300).ConfigureAwait(false);
        
        Console.WriteLine("[Wrapper] 資源已安全且非同步地釋放");
    }
}
