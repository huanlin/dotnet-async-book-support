Console.WriteLine("示範第 7 章：使用 ParallelOptions 控制平行迴圈");
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
                Console.WriteLine($"[執行緒 {Environment.CurrentManagedThreadId}] 開始處理第 {item} 筆，目前同時執行數: {currentWorkers}");
                DoCpuBoundWork(item);

                if (item == 6)
                {
                    Console.WriteLine($"[執行緒 {Environment.CurrentManagedThreadId}] 處理到第 {item} 筆，送出取消要求...");
                    cts.Cancel();
                }

                options.CancellationToken.ThrowIfCancellationRequested();
                Console.WriteLine($"[執行緒 {Environment.CurrentManagedThreadId}] 已完成處理第 {item} 筆");
            }
            finally
            {
                Interlocked.Decrement(ref activeWorkers);
            }
        });
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("平行迴圈已收到取消訊號並結束。");
    }

    Console.WriteLine($"觀察到的最大同時執行數: {maxObservedWorkers}");
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

        if (Interlocked.CompareExchange(ref currentMax, candidate, snapshot) == snapshot)
        {
            return;
        }
    }
}
