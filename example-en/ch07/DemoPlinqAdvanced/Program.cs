using System.Diagnostics;

Console.WriteLine("Demo: advanced PLINQ control and merge options");

List<int> numbers = Enumerable.Range(1, 20).ToList();

Console.WriteLine("\n--- Example 1: FullyBuffered ---");
// FullyBuffered waits for all workers to finish all work
// before returning any results to the caller.
// Benefit: can be good for overall throughput in some workloads.
// Trade-off: the first result has the highest latency.
RunPlinqWithMergeOption(numbers, ParallelMergeOptions.FullyBuffered);

Console.WriteLine("\n--- Example 2: NotBuffered ---");
// NotBuffered returns a result to the caller as soon as any worker computes one.
// Benefit: very low latency for the first result.
// Trade-off: more frequent handoff can reduce overall throughput.
RunPlinqWithMergeOption(numbers, ParallelMergeOptions.NotBuffered);

// ======= Method definitions =======

void RunPlinqWithMergeOption(List<int> source, ParallelMergeOptions option)
{
    var sw = Stopwatch.StartNew();

    // Build a PLINQ query with the desired merge behavior.
    var query = source.AsParallel()
                      .WithMergeOptions(option)
                      .Select(x =>
                      {
                          int result = ComputeExpensiveSquare(x);

                          // This output is only for demonstration:
                          // it shows when worker threads produce results,
                          // not when those results have been merged back to the consumer.
                          // In real code, avoid side effects inside the query.
                          Console.Write($"[Worker produced {result}] ");
                          return result;
                      });

    // Iterate through the query results one by one.
    // Because we use foreach here, PLINQ must merge results from multiple workers
    // back into the single thread that is consuming this enumeration.
    // That is where the merge option matters.
    //
    // Note: if we used .ForAll() instead, the results would be processed directly
    // on the worker threads, so there would be no merge-back step at all.
    foreach (var result in query)
    {
        Console.Write($"[Consumer received {result}] ");
    }

    Console.WriteLine($"\nTotal time: {sw.ElapsedMilliseconds} ms");
}

static int ComputeExpensiveSquare(int value)
{
    double score = 0;
    int iterations = 1_000_000 + value * 10_000;

    for (int i = 1; i <= iterations; i++)
    {
        score += Math.Sqrt(i + value);
    }

    GC.KeepAlive(score);
    return value * value;
}
