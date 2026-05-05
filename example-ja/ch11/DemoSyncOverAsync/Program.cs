using System.Collections.Concurrent;

Console.WriteLine("Sync-over-Async アンチパターンのデモ");

var api = new BadSyncWrapperApi();
var completed = false;
Exception? capturedException = null;

var thread = new Thread(() =>
{
    SynchronizationContext.SetSynchronizationContext(new QueuedSynchronizationContext());

    try
    {
        Console.WriteLine("[Worker] 同期ラップされた GetData() を呼び出そうとしています...");
        string result = api.GetData();
        Console.WriteLine($"[Worker] 結果を取得しました: {result}");
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
    Console.WriteLine("ワーカー スレッドが完了前にタイムアウトしたことを検出しました。");
    Console.WriteLine("これは通常、継続が同じ SynchronizationContext に戻ろうとしている間に、.Result が現在のスレッドをブロックしたことを意味します。");
    Console.WriteLine("これこそ、Sync-over-Async が UI や単一スレッド環境でデッドロックを起こしやすい理由です。");
}
else if (capturedException is not null)
{
    Console.WriteLine($"ワーカー スレッドが例外をスローしました: {capturedException.GetType().Name} - {capturedException.Message}");
}
else if (completed)
{
    Console.WriteLine("この実行は予期せず完了しました。UI や単一スレッド環境では、それでもデッドロックする可能性があります。");
}

public sealed class BadSyncWrapperApi
{
    public string GetData()
    {
        // アンチパターン: 同期 API の中で非同期メソッドをブロックして待つ
        return GetDataAsync().Result;
    }

    public async Task<string> GetDataAsync()
    {
        Console.WriteLine("[API] 非同期操作を開始します。後で元のコンテキストで再開しようとします。");
        await Task.Delay(300);
        return "こんにちは";
    }
}

public sealed class QueuedSynchronizationContext : SynchronizationContext
{
    private readonly BlockingCollection<(SendOrPostCallback Callback, object? State)> _queue = [];

    public override void Post(SendOrPostCallback d, object? state)
    {
        _queue.Add((d, state));
        Console.WriteLine("[SyncContext] 継続はキューに入りましたが、現在のスレッドは .Result でブロックされています。");
    }
}
