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

Console.WriteLine("\n按下 Enter 鍵結束...");
Console.ReadLine();

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
    // 模擬一個耗時 1 秒的 CPU 密集型工作
    Thread.Sleep(1000);
}
