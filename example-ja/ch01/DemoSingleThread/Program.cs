// ステージ 1: 同期的な 1 人のシェフ (同期シングルスレッド)
// 1 本のスレッドがどのようにブロックされるかを示す

using System.Diagnostics;

Console.WriteLine("ピザ店が開店しました! ステージ 1: シェフは 1 人だけ (シングルスレッド)");
var sw = Stopwatch.StartNew();

// 3 枚のピザを 1 枚ずつ順番に作る様子をシミュレートする
MakePizza(1);
MakePizza(2);
MakePizza(3);

sw.Stop();
Console.WriteLine($"すべてのピザが完成しました。合計時間: {sw.ElapsedMilliseconds} ms");

void MakePizza(int id)
{
    Console.WriteLine($"[1 人シェフ] ピザ {id} の生地の準備を始めます...");
    Thread.Sleep(500); // 刻んだりこねたりする下準備の時間をシミュレートする

    Console.WriteLine($"[1 人シェフ] ピザ {id} をオーブンに入れました。待機中...");

    // ここでは Thread.Sleep でブロッキング操作をシミュレートする。
    // 次のピザの準備も、電話対応もできない。
    Thread.Sleep(2000);

    Console.WriteLine($"[1 人シェフ] ピザ {id} が焼き上がりました! 取り出します。");
}
