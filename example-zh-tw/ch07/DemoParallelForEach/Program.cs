using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("示範 Parallel.ForEach 與傳統 foreach 的效能差異");
Console.WriteLine($"當前系統核心數: {Environment.ProcessorCount}\n");

// 準備 8 個虛擬檔案
var files = Enumerable.Range(1, 8).Select(i => $"File_{i}.txt").ToList();

// 1. 循序處理
ProcessFilesSequentially(files);

Console.WriteLine();

// 2. 平行處理
ProcessFilesParallel(files);

static void ProcessFilesSequentially(IEnumerable<string> files)
{
    Console.WriteLine("開始循序處理檔案...");
    var sw = Stopwatch.StartNew();
    foreach (var file in files)
    {
        ProcessSingleFile(file);
    }
    sw.Stop();
    Console.WriteLine($"循序處理完成，耗時: {sw.ElapsedMilliseconds} ms");
}

static void ProcessFilesParallel(IEnumerable<string> files)
{
    Console.WriteLine("開始平行處理檔案...");
    var sw = Stopwatch.StartNew();
    Parallel.ForEach(files, file =>
    {
        ProcessSingleFile(file);
    });
    sw.Stop();
    Console.WriteLine($"平行處理完成，耗時: {sw.ElapsedMilliseconds} ms");
}

static void ProcessSingleFile(string file)
{
    // 模擬 CPU 密集型工作：重複執行大量數值運算
    double score = 0;
    int iterations = 15_000_000 + file.Length * 100_000;

    for (int i = 1; i <= iterations; i++)
    {
        score += Math.Sqrt(i + file.Length);
    }

    GC.KeepAlive(score);
}
