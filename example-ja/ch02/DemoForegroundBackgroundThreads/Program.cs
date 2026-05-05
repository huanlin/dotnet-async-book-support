using System.Threading;
using System.Threading.Tasks;

// 例 1: Thread を使う (フォアグラウンド スレッド)
var foregroundThread = new Thread(() =>
{
    Thread.Sleep(3000);
    Console.WriteLine("フォアグラウンド スレッドが終了しました。");
});
// foregroundThread.IsBackground = true; // 手動でバックグラウンドに設定できる
foregroundThread.Start();
Console.WriteLine("Main メソッド (フォアグラウンド) は終了しようとしていますが、プログラムはフォアグラウンド スレッドの終了を待ちます。");


// 例 2: Task.Run を使う (バックグラウンド スレッド)
_ = Task.Run(() =>
{
    Thread.Sleep(5000);
    // この行は実行されない可能性がある
    Console.WriteLine("バックグラウンド スレッドが終了しました。");
});
Console.WriteLine("Main メソッド (フォアグラウンド) は終了しようとしており、プログラムはバックグラウンド スレッドを待ちません。");
