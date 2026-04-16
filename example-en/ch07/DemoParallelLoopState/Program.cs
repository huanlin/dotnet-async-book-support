Console.WriteLine("Demo: control a parallel loop with ParallelLoopState");

Console.WriteLine(
    "\n--- Example 1: use Stop() to stop not-yet-started iterations as soon as possible ---");
RunParallelLoopWithStop();

Console.WriteLine(
    "\n--- Example 2: use Break() to ensure lower-index iterations still finish ---");
RunParallelLoopWithBreak();

void RunParallelLoopWithStop()
{
    // Once Stop() is called, the loop tries to reject any new iterations
    // that have not started yet as quickly as possible.
    // Even iterations with an index smaller than the one that called Stop()
    // may be skipped if they had not started yet.
    ParallelLoopResult result = Parallel.For(1, 15, (i, state) =>
    {
        if (i == 5)
        {
            Console.WriteLine(
                $"[Thread {Environment.CurrentManagedThreadId}] Reached item {i}. Calling state.Stop() to stop iterations that have not started yet as soon as possible!");
            state.Stop();
        }

        // Simulate a short processing time.
        Thread.Sleep(200);
        Console.WriteLine(
            $"[Thread {Environment.CurrentManagedThreadId}] Finished item {i}");
    });

    Console.WriteLine($"Did the loop complete fully? {result.IsCompleted}");
    if (!result.IsCompleted)
    {
        Console.WriteLine(
            $"Note: once Stop() is called, LowestBreakIteration is always null: {result.LowestBreakIteration == null}");
    }
}

void RunParallelLoopWithBreak()
{
    // Once Break() is called, the loop guarantees that all iterations
    // with an index smaller than the current one will still run to completion.
    ParallelLoopResult result = Parallel.For(1, 15, (i, state) =>
    {
        if (i == 5)
        {
            Console.WriteLine(
                $"[Thread {Environment.CurrentManagedThreadId}] Reached item {i}. Calling state.Break() to wind the loop down early!");
            state.Break();
        }

        // Simulate a short processing time.
        Thread.Sleep(200);
        Console.WriteLine(
            $"[Thread {Environment.CurrentManagedThreadId}] Finished item {i}");
    });

    Console.WriteLine($"Did the loop complete fully? {result.IsCompleted}");
    if (!result.IsCompleted)
    {
        Console.WriteLine(
            $"The loop was interrupted by Break. The earliest iteration that requested it was: {result.LowestBreakIteration}");
    }
}
