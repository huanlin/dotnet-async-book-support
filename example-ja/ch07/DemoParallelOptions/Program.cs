Console.WriteLine("デモ: ParallelOptions で並列ループを制御します");
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
                    $"[スレッド {Environment.CurrentManagedThreadId}] 項目 {item} の処理を開始します。現在の並行数: {currentWorkers}");
                DoCpuBoundWork(item);

                if (item == 6)
                {
                    Console.WriteLine(
                        $"[スレッド {Environment.CurrentManagedThreadId}] 項目 {item} に到達しました。キャンセル要求を送信します...");
                    cts.Cancel();
                }

                options.CancellationToken.ThrowIfCancellationRequested();
                Console.WriteLine(
                    $"[スレッド {Environment.CurrentManagedThreadId}] 項目 {item} が完了しました");
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
            "並列ループはキャンセル信号を受け取り、終了しました。");
    }

    Console.WriteLine(
        $"観測された最大並行数: {maxObservedWorkers}");
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
