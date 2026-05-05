using System.Diagnostics;

Console.WriteLine("デモ: PLINQ の高度な制御とマージ オプション");

List<int> numbers = Enumerable.Range(1, 20).ToList();

Console.WriteLine("\n--- 例 1: FullyBuffered ---");
// FullyBuffered はすべてのワーカーが全作業を終えるまで待つ
// その後で呼び出し元へ結果を返す。
// 利点: ワークロードによっては全体のスループットに有利。
// トレードオフ: 最初の結果のレイテンシが最も高い。
RunPlinqWithMergeOption(numbers, ParallelMergeOptions.FullyBuffered);

Console.WriteLine("\n--- 例 2: NotBuffered ---");
// NotBuffered は、いずれかのワーカーが結果を計算した時点ですぐ呼び出し元へ返す。
// 利点: 最初の結果のレイテンシが非常に低い。
// トレードオフ: 受け渡しが頻繁になるため、全体のスループットが下がることがある。
RunPlinqWithMergeOption(numbers, ParallelMergeOptions.NotBuffered);

// ======= メソッド定義 =======

void RunPlinqWithMergeOption(List<int> source, ParallelMergeOptions option)
{
    var sw = Stopwatch.StartNew();

    // 目的のマージ動作を持つ PLINQ クエリを組み立てる。
    var query = source.AsParallel()
                      .WithMergeOptions(option)
                      .Select(x =>
                      {
                          int result = ComputeExpensiveSquare(x);

                          // この出力はデモ用である:
                          // ワーカー スレッドがいつ結果を生成したかを示すものであり、
                          // それらの結果がいつコンシューマーへマージされたかを示すものではない。
                          // 実際のコードでは、クエリ内の副作用を避ける。
                          Console.Write($"[ワーカーが {result} を生成] ");
                          return result;
                      });

    // クエリ結果を 1 つずつ反復する。
    // ここでは foreach を使うため、PLINQ は複数ワーカーからの結果を
    // この列挙を消費している単一スレッドへマージし直す必要がある。
    // そこでマージ オプションが重要になる。
    //
    // 補足: 代わりに .ForAll() を使うと、結果は直接
    // ワーカー スレッド上で処理されるため、マージし直すステップは存在しない。
    foreach (var result in query)
    {
        Console.Write($"[コンシューマーが {result} を受信] ");
    }

    Console.WriteLine($"\n合計時間: {sw.ElapsedMilliseconds} ms");
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
