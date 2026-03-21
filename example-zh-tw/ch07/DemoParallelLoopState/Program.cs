Console.WriteLine("示範第 7 章：使用 ParallelLoopState 控制平行迴圈");

Console.WriteLine("\n--- 展示一：使用 Stop() 要求盡快停止尚未開始的迭代 ---");
RunParallelLoopWithStop();

Console.WriteLine("\n--- 展示二：使用 Break() 確保當前索引之前的迭代完成 ---");
RunParallelLoopWithBreak();

void RunParallelLoopWithStop()
{
    // Stop() 的特性：一旦呼叫，迴圈會盡快拒絕任何「尚未開始」的新迭代
    // 即使該迭代的索引是排在呼叫 Stop() 的索引「之前」，也有可能被放棄執行。
    ParallelLoopResult result = Parallel.For(1, 15, (i, state) =>
    {
        if (i == 5)
        {
            Console.WriteLine($"[執行緒 {Environment.CurrentManagedThreadId}] 處理到第 {i} 筆，呼叫 state.Stop()，要求盡快停止尚未開始的迭代！");
            state.Stop();
        }

        // 模擬短暫的處理時間
        Thread.Sleep(200);
        Console.WriteLine($"[執行緒 {Environment.CurrentManagedThreadId}] 已完成處理第 {i} 筆");
    });

    Console.WriteLine($"迴圈已完成? {result.IsCompleted}");
    if (!result.IsCompleted)
    {
        Console.WriteLine($"注意: Stop() 被呼叫，LowestBreakIteration 永遠會是 null: {result.LowestBreakIteration == null}");
    }
}

void RunParallelLoopWithBreak()
{
    // Break() 的特性：一旦呼叫，它會承諾讓所有「早於」當前索引的迭代完整跑完
    // (例如在 i=5 呼叫 Break，它保證 i=1,2,3,4 一定會全部做完)
    ParallelLoopResult result = Parallel.For(1, 15, (i, state) =>
    {
        if (i == 5)
        {
            Console.WriteLine($"[執行緒 {Environment.CurrentManagedThreadId}] 處理到第 {i} 筆，呼叫 state.Break() 要求提早收尾！");
            state.Break();
        }

        // 模擬短暫的處理時間
        Thread.Sleep(200);
        Console.WriteLine($"[執行緒 {Environment.CurrentManagedThreadId}] 已完成處理第 {i} 筆");
    });

    Console.WriteLine($"迴圈已完成? {result.IsCompleted}");
    if (!result.IsCompleted)
    {
        Console.WriteLine($"迴圈被 Break 中斷，最早發起中斷的迭代索引是: {result.LowestBreakIteration}");
    }
}
