using System.Collections.Concurrent;

Console.WriteLine("Demonstrating the Sync-over-Async anti-pattern");

var api = new BadSyncWrapperApi();
var completed = false;
Exception? capturedException = null;

var thread = new Thread(() =>
{
    SynchronizationContext.SetSynchronizationContext(new QueuedSynchronizationContext());

    try
    {
        Console.WriteLine("[Worker] About to call the synchronously wrapped GetData()...");
        string result = api.GetData();
        Console.WriteLine($"[Worker] Retrieved the result: {result}");
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
    Console.WriteLine("Detected that the worker thread timed out before completing.");
    Console.WriteLine("This usually means .Result blocked the current thread while the continuation wanted to get back to the same SynchronizationContext.");
    Console.WriteLine("That is exactly why Sync-over-Async easily causes deadlocks in UI or single-threaded environments.");
}
else if (capturedException is not null)
{
    Console.WriteLine($"The worker thread threw an exception: {capturedException.GetType().Name} - {capturedException.Message}");
}
else if (completed)
{
    Console.WriteLine("This run completed unexpectedly; in a UI or single-threaded environment, it could still deadlock.");
}

public sealed class BadSyncWrapperApi
{
    public string GetData()
    {
        // Anti-pattern: block on an asynchronous method inside a synchronous API
        return GetDataAsync().Result;
    }

    public async Task<string> GetDataAsync()
    {
        Console.WriteLine("[API] Starting the asynchronous operation; it will try to resume on the original context later.");
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
        Console.WriteLine("[SyncContext] The continuation has been queued, but the current thread is blocked by .Result.");
    }
}
