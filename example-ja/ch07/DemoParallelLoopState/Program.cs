Console.WriteLine("デモ: ParallelLoopState で並列ループを制御します");

Console.WriteLine(
    "\n--- 例 1: Stop() で未開始の反復をできるだけ早く止める ---");
RunParallelLoopWithStop();

Console.WriteLine(
    "\n--- 例 2: Break() で小さいインデックスの反復は完了させる ---");
RunParallelLoopWithBreak();

void RunParallelLoopWithStop()
{
    // Stop() が呼ばれると、ループは新しい反復をできるだけ早く拒否しようとする
    // まだ開始されていない反復を拒否する。
    // Stop() を呼んだ反復より小さいインデックスの反復でも、
    // まだ開始されていなければスキップされる可能性がある。
    ParallelLoopResult result = Parallel.For(1, 15, (i, state) =>
    {
        if (i == 5)
        {
            Console.WriteLine(
                $"[スレッド {Environment.CurrentManagedThreadId}] 項目 {i} に到達しました。未開始の反復をできるだけ早く止めるために state.Stop() を呼び出します!");
            state.Stop();
        }

        // 短い処理時間をシミュレートする。
        Thread.Sleep(200);
        Console.WriteLine(
            $"[スレッド {Environment.CurrentManagedThreadId}] 項目 {i} が完了しました");
    });

    Console.WriteLine($"ループは完全に完了しましたか? {result.IsCompleted}");
    if (!result.IsCompleted)
    {
        Console.WriteLine(
            $"Note: Stop() が呼び出されると、LowestBreakIteration は常に null です: {result.LowestBreakIteration == null}");
    }
}

void RunParallelLoopWithBreak()
{
    // Break() が呼ばれると、ループはすべての反復について
    // 現在より小さいインデックスの反復が最後まで実行されることを保証する。
    ParallelLoopResult result = Parallel.For(1, 15, (i, state) =>
    {
        if (i == 5)
        {
            Console.WriteLine(
                $"[スレッド {Environment.CurrentManagedThreadId}] 項目 {i} に到達しました。ループを早めに終息させるために state.Break() を呼び出します!");
            state.Break();
        }

        // 短い処理時間をシミュレートする。
        Thread.Sleep(200);
        Console.WriteLine(
            $"[スレッド {Environment.CurrentManagedThreadId}] 項目 {i} が完了しました");
    });

    Console.WriteLine($"ループは完全に完了しましたか? {result.IsCompleted}");
    if (!result.IsCompleted)
    {
        Console.WriteLine(
            $"ループは Break によって中断されました。最初に要求した反復: {result.LowestBreakIteration}");
    }
}
