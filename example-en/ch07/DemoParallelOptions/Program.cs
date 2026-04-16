Console.WriteLine("Demo: control a parallel loop with ParallelOptions");
RunParallelLoopWithOptions();

void RunParallelLoopWithOptions()
{
    using var cts = new CancellationTokenSource();
    var options = new ParallelOptions
    {
        MaxDegreeOfParallelism = 2,
        CancellationToken = cts.Token
    };

    int activeWorkers = 0;
    int maxObservedWorkers = 0;

    try
    {
        Parallel.ForEach(Enumerable.Range(1, 10), options, item =>
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            int currentWorkers = Interlocked.Increment(ref activeWorkers);
            UpdateMaxConcurrency(ref maxObservedWorkers, currentWorkers);

            try
            {
                Console.WriteLine(
                    $"[Thread {Environment.CurrentManagedThreadId}] Start processing item {item}. Current concurrency: {currentWorkers}");
                DoCpuBoundWork(item);

                if (item == 6)
                {
                    Console.WriteLine(
                        $"[Thread {Environment.CurrentManagedThreadId}] Reached item {item}. Sending a cancellation request...");
                    cts.Cancel();
                }

                options.CancellationToken.ThrowIfCancellationRequested();
                Console.WriteLine(
                    $"[Thread {Environment.CurrentManagedThreadId}] Finished item {item}");
            }
            finally
            {
                Interlocked.Decrement(ref activeWorkers);
            }
        });
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine(
            "The parallel loop received the cancellation signal and ended.");
    }

    Console.WriteLine(
        $"Maximum observed concurrency: {maxObservedWorkers}");
}

static void DoCpuBoundWork(int value)
{
    double score = 0;
    int iterations = 4_000_000 + value * 100_000;

    for (int i = 1; i <= iterations; i++)
    {
        score += Math.Sqrt(i + value);
    }

    GC.KeepAlive(score);
}

static void UpdateMaxConcurrency(ref int currentMax, int candidate)
{
    while (true)
    {
        int snapshot = currentMax;
        if (candidate <= snapshot)
        {
            return;
        }

        if (Interlocked.CompareExchange(
            ref currentMax, candidate, snapshot) == snapshot)
        {
            return;
        }
    }
}
