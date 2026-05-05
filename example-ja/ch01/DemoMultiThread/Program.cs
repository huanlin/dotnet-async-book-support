// ステージ 2: より柔軟に複数作業をこなすシェフたち (マルチスレッド)
// 複数のスレッドによる並行作業を示す

using System.Diagnostics;

Console.WriteLine("ピザ店が開店しました! ステージ 2: 複数のシェフが作業中 (マルチスレッド)");
var sw = Stopwatch.StartNew();

// 3 人のシェフに対応する 3 本のスレッドを作成する
Thread chef1 = new(() => MakePizza(1));
Thread chef2 = new(() => MakePizza(2));
Thread chef3 = new(() => MakePizza(3));

// すべてのシェフを同時に開始する
chef1.Start();
chef2.Start();
chef3.Start();

// メイン スレッド (店長) は、すべてのシェフが終わるのを待つ
chef1.Join();
chef2.Join();
chef3.Join();

sw.Stop();
Console.WriteLine($"すべてのピザが完成しました。合計時間: {sw.ElapsedMilliseconds} ms");

void MakePizza(int id)
{
    int threadId = Environment.CurrentManagedThreadId;

    Console.WriteLine($"[シェフ {threadId}] ピザ {id} の準備を始めます...");
    Thread.Sleep(500); // 刻んだりこねたりする下準備の時間をシミュレートする

    Console.WriteLine($"[シェフ {threadId}] ピザ {id} をオーブンに入れました。待機中...");

    // Thread.Sleep はここでもブロッキング操作だが、各ピザには専属の
    // 専属のシェフ (スレッド) がいるため、1 人のシェフが待っている間も、ほかのシェフは
    // 自分のピザの作業を続けられる。
    Thread.Sleep(2000);

    Console.WriteLine($"[シェフ {threadId}] ピザ {id} が焼き上がりました! 取り出します。");
}
