using System.Diagnostics;

Console.WriteLine("Demonstrating Chapter 9: Thread Pool Starvation");
Console.WriteLine("Note: this example intentionally creates starvation, so it may take tens of seconds to finish.");

// To make the effect more obvious, intentionally lower the ThreadPool minimum thread count.
// That makes the initial workforce far smaller than the sudden burst of incoming work,
// which amplifies the starvation effect.
ThreadPool.SetMinThreads(1, 1);

// Simulate 500 concurrent HTTP requests arriving all at once
int requestCount = 500;
var tasks = new Task[requestCount];
bool useProperAsyncVersion = false;

var sw = Stopwatch.StartNew();

for (int i = 0; i < requestCount; i++)
{
    int requestId = i;
    // Each request is a work item running on the ThreadPool

    // By default, use sync-over-async to demonstrate the delays caused by the wrong approach.
    // To see the healthy async version instead, change useProperAsyncVersion to true.
    tasks[i] = useProperAsyncVersion
        ? Task.Run(() => ProcessRequestProperlyAsync(requestId))
        : Task.Run(() => ProcessRequestSyncOverAsync(requestId));
}

await Task.WhenAll(tasks);
sw.Stop();

Console.WriteLine($"\nAll {requestCount} requests have been processed!");
Console.WriteLine($"Total elapsed time: {sw.ElapsedMilliseconds} ms");

// --- Incorrect example: sync-over-async that causes starvation ---
void ProcessRequestSyncOverAsync(int id)
{
    // [Fatal mistake] Synchronously wait for an async method on a background thread.
    // This ties up a valuable ThreadPool thread for about one second, preventing it from serving other requests.
    // In this example, we queue 500 work items at once while setting the minimum thread count to 1,
    // so the ThreadPool has to fight fires while slowly adding threads, which stretches the overall latency badly.
    
    LogThreadCount(id, "Start");
    
    var result = SimulateDatabaseQueryAsync().Result; // <-- the root of the disaster
    
    LogThreadCount(id, "End");
}

// --- Correct example: async all the way ---
async Task ProcessRequestProperlyAsync(int id)
{
    // [Good practice] await immediately returns the thread to the ThreadPool while waiting for I/O.
    // Even if 500 requests arrive at once, threads do not stay blocked in a waiting state,
    // so the system can usually keep a large amount of I/O work moving with only a small number of threads.
    
    LogThreadCount(id, "Start");
    
    var result = await SimulateDatabaseQueryAsync(); // <-- releases the thread
    
    LogThreadCount(id, "End");
}

async Task<string> SimulateDatabaseQueryAsync()
{
    // Simulate network I/O or a database query that takes one second
    await Task.Delay(1000);
    return "Data";
}

void LogThreadCount(int id, string state)
{
    ThreadPool.GetAvailableThreads(out int workerThreads, out _);
    ThreadPool.GetMaxThreads(out int maxWorkerThreads, out _);
    int activeThreads = maxWorkerThreads - workerThreads;
    
    Console.WriteLine($"[Request {id:D3}] {state} - Active pool threads: {activeThreads}");
}
