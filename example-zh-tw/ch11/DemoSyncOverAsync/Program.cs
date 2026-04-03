using System.Collections.Concurrent;

Console.WriteLine("示範 Sync-over-Async 反模式");

var api = new BadSyncWrapperApi();
var completed = false;
Exception? capturedException = null;

var thread = new Thread(() =>
{
    SynchronizationContext.SetSynchronizationContext(new QueuedSynchronizationContext());

    try
    {
        Console.WriteLine("[Worker] 準備呼叫同步包裝的 GetData()...");
        string result = api.GetData();
        Console.WriteLine($"[Worker] 取得結果：{result}");
        completed = true;
    }
    catch (Exception ex)
    {
        capturedException = ex;
        completed = true;
    }
})
{
    IsBackground = true
};

thread.Start();

if (!thread.Join(1500))
{
    Console.WriteLine("偵測到工作執行緒超時未完成。");
    Console.WriteLine("這通常表示 .Result 卡住了目前執行緒，而 continuation 又想切回同一個 SynchronizationContext。");
    Console.WriteLine("這正是 Sync-over-Async 在 UI / 單執行緒環境中容易引發死鎖的原因。");
}
else if (capturedException is not null)
{
    Console.WriteLine($"工作執行緒拋出例外：{capturedException.GetType().Name} - {capturedException.Message}");
}
else if (completed)
{
    Console.WriteLine("本次執行意外完成；若換成 UI/單執行緒環境，仍可能發生死鎖。");
}

public sealed class BadSyncWrapperApi
{
    public string GetData()
    {
        // 反模式：在同步 API 中阻塞等待非同步方法
        return GetDataAsync().Result;
    }

    public async Task<string> GetDataAsync()
    {
        Console.WriteLine("[API] 開始非同步作業，稍後要切回原 context。");
        await Task.Delay(300);
        return "Hello";
    }
}

public sealed class QueuedSynchronizationContext : SynchronizationContext
{
    private readonly BlockingCollection<(SendOrPostCallback Callback, object? State)> _queue = [];

    public override void Post(SendOrPostCallback d, object? state)
    {
        _queue.Add((d, state));
        Console.WriteLine("[SyncContext] continuation 已被排入佇列，但目前執行緒正被 .Result 卡住。");
    }
}
