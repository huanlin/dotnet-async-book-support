using System.Diagnostics;

Console.WriteLine("示範第 7 章：PLINQ 的進階控制與合併選項");

List<int> numbers = Enumerable.Range(1, 20).ToList();

Console.WriteLine("\n--- 展示一：使用 FullyBuffered (全緩衝) ---");
// FullyBuffered 會等到所有執行緒都做完所有工作，才一次把結果交給消費端
// 優點：整體系統吞吐量 (Throughput) 最高
// 缺點：第一個結果出現的潛伏期 (Latency) 最長
RunPlinqWithMergeOption(numbers, ParallelMergeOptions.FullyBuffered);

Console.WriteLine("\n--- 展示二：使用 NotBuffered (無緩衝) ---");
// NotBuffered 只要有任何一個執行緒算出一個結果，就會立刻交給消費端
// 優點：潛伏期極短，能立刻看到第一個結果
// 缺點：頻繁的交接可能導致整體吞吐量降低
RunPlinqWithMergeOption(numbers, ParallelMergeOptions.NotBuffered);

// ======= 方法定義 =======

void RunPlinqWithMergeOption(List<int> source, ParallelMergeOptions option)
{
    var sw = Stopwatch.StartNew();

    // 建立 PLINQ 查詢，指定我們想要的合併方式
    var query = source.AsParallel()
                      .WithMergeOptions(option)
                      .Select(x =>
                      {
                          // 模擬一個比較耗時的運算
                          Thread.Sleep(100); 
                          Console.Write($"[產出 {Math.Pow(x, 2)}] ");
                          return Math.Pow(x, 2);
                      });

    // 開始消費這個查詢的結果
    // 因為我們用了 foreach 迴圈，PLINQ 引擎必須負責將各個不同執行緒的結果
    // 重新「合併 (Merge)」回這個主執行緒上供迴圈消費。
    // 這時候 WithMergeOptions 的設定就會決定它合併的緩衝策略。
    
    // 注意：如果是用 .ForAll()，結果會直接在各個 Worker 執行緒上被消費掉，
    // 根本不存在合併回主執行緒的步驟，那 WithMergeOptions 就毫無意義了。
    
    foreach (var result in query)
    {
        Console.Write($"{result} ");
    }
    
    Console.WriteLine($"\n總耗時: {sw.ElapsedMilliseconds} ms");
}
