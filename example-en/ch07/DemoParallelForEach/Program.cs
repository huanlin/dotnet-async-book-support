using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("Demo: compare Parallel.ForEach with a traditional foreach loop");
Console.WriteLine($"Current processor count: {Environment.ProcessorCount}\n");

// Prepare 8 dummy files.
var files = Enumerable.Range(1, 8).Select(i => $"File_{i}.txt").ToList();

RunSequentialDemo(files);

Console.WriteLine();

RunParallelDemo(files);

static void RunSequentialDemo(IReadOnlyList<string> files)
{
    Console.WriteLine("=== Sequential version ===");
    ProcessFilesSequentially(files);
}

static void RunParallelDemo(IReadOnlyList<string> files)
{
    Console.WriteLine("=== Parallel version ===");
    ProcessFilesParallel(files);
}

static void ProcessFilesSequentially(IEnumerable<string> files)
{
    Console.WriteLine("Starting sequential file processing...");
    var sw = Stopwatch.StartNew();
    foreach (var file in files)
    {
        ProcessSingleFile(file);
    }
    sw.Stop();
    Console.WriteLine(
        $"Sequential processing finished in {sw.ElapsedMilliseconds} ms");
}

static void ProcessFilesParallel(IEnumerable<string> files)
{
    Console.WriteLine("Starting parallel file processing...");
    var sw = Stopwatch.StartNew();
    Parallel.ForEach(files, file => ProcessSingleFile(file));
    sw.Stop();
    Console.WriteLine(
        $"Parallel processing finished in {sw.ElapsedMilliseconds} ms");
}

static void ProcessSingleFile(string file)
{
    // Simulate CPU-bound work by repeating a large amount of numeric computation.
    double score = 0;
    int iterations = 15_000_000 + file.Length * 100_000;

    for (int i = 1; i <= iterations; i++)
    {
        score += Math.Sqrt(i + file.Length);
    }

    GC.KeepAlive(score);
}
