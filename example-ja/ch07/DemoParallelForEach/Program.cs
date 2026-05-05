using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("デモ: Parallel.ForEach と従来の foreach ループを比較します");
Console.WriteLine($"現在のプロセッサ数: {Environment.ProcessorCount}\n");

// ダミー ファイルを 8 個用意する。
var files = Enumerable.Range(1, 8).Select(i => $"File_{i}.txt").ToList();

RunSequentialDemo(files);

Console.WriteLine();

RunParallelDemo(files);

static void RunSequentialDemo(IReadOnlyList<string> files)
{
    Console.WriteLine("=== 逐次版 ===");
    ProcessFilesSequentially(files);
}

static void RunParallelDemo(IReadOnlyList<string> files)
{
    Console.WriteLine("=== 並列版 ===");
    ProcessFilesParallel(files);
}

static void ProcessFilesSequentially(IEnumerable<string> files)
{
    Console.WriteLine("逐次ファイル処理を開始します...");
    var sw = Stopwatch.StartNew();
    foreach (var file in files)
    {
        ProcessSingleFile(file);
    }
    sw.Stop();
    Console.WriteLine(
        $"逐次処理が {sw.ElapsedMilliseconds} ms で完了しました");
}

static void ProcessFilesParallel(IEnumerable<string> files)
{
    Console.WriteLine("並列ファイル処理を開始します...");
    var sw = Stopwatch.StartNew();
    Parallel.ForEach(files, file => ProcessSingleFile(file));
    sw.Stop();
    Console.WriteLine(
        $"並列処理が {sw.ElapsedMilliseconds} ms で完了しました");
}

static void ProcessSingleFile(string file)
{
    // 大量の数値計算を繰り返して、CPU バウンドな作業をシミュレートする。
    double score = 0;
    int iterations = 15_000_000 + file.Length * 100_000;

    for (int i = 1; i <= iterations; i++)
    {
        score += Math.Sqrt(i + file.Length);
    }

    GC.KeepAlive(score);
}
