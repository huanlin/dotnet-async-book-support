// ステージ 3: 超効率的なスマート シェフ (非同期プログラミング)
// async/await がノンブロッキング待機のためにスレッドを解放する様子を示す

using System.Diagnostics;

var msg = "ピザ店が開店しました! ステージ 3: スマート オーブンを使うスーパーシェフ (非同期プログラミング)";
Console.WriteLine(msg);
var sw = Stopwatch.StartNew();

// ピザ作りの非同期タスクを 3 つ開始する
// 補足: async メソッドを呼び出すと、まず現在のスレッド上で実行が始まる
// そして、最初の未完了の await に到達するまで実行される。
// 開始時点で自動的に新しいバックグラウンド スレッドへ移るわけではない。
var p1 = MakePizzaAsync(1);
var p2 = MakePizzaAsync(2);
var p3 = MakePizzaAsync(3);

// すべてのピザ タスクが終わるのを非同期に待つ
await Task.WhenAll(p1, p2, p3);

sw.Stop();
msg = $"すべてのピザが完成しました。合計時間: {sw.ElapsedMilliseconds} ms";
Console.WriteLine(msg);

async Task MakePizzaAsync(int id)
{
    // スレッドの変化を観察できるように、現在のスレッド ID を取得する
    int threadId = Environment.CurrentManagedThreadId;
    var str = $"[シェフ {threadId}] ピザ {id} を開始します。まず生地がふくらむのを待ちます...";
    Console.WriteLine(str);

    // 生地がふくらむのを待つ場合など、非同期に待機できる状況をシミュレートする。
    // 材料が届くのを待つ場合も同じで、これはスレッドをブロックしない。
    await Task.Delay(500);

    threadId = Environment.CurrentManagedThreadId;
    str = $"[シェフ {threadId}] 生地の準備ができました。ピザ {id} をオーブンに入れてタイマーをセットし、別の作業に移ります!";
    Console.WriteLine(str);

    // 外部からの応答を待つ、時間のかかる処理をシミュレートする。
    // ここではオーブンで焼く処理に相当する。await 中、スレッドはブロックされない。
    // 待機が終わるまで、システムはそのスレッドをほかの作業に使える。
    await Task.Delay(2000);

    threadId = Environment.CurrentManagedThreadId;
    str = $"[シェフ {threadId}] チーン! ピザ {id} が焼き上がりました。戻って取り出します。";
    Console.WriteLine(str);
}
