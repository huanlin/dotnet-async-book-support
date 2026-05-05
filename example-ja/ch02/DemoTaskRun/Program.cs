using System.Threading;
using System.Threading.Tasks;

Console.WriteLine($"メイン スレッド ID: {Environment.CurrentManagedThreadId}");

Console.WriteLine("Task.Run でバックグラウンド作業を開始する準備をしています...");

// 作業をスレッド プールへ渡し、Task オブジェクトを受け取る
Task task = Task.Run(() =>
{
    Console.WriteLine($"バックグラウンド スレッド ID: {Environment.CurrentManagedThreadId}");
    Console.WriteLine("バックグラウンド作業を実行中です...");
    Thread.Sleep(2000); // ここでは、時間のかかる同期処理をシミュレートするためだけに使う
    Console.WriteLine("バックグラウンド作業が完了しました。");
});

Console.WriteLine("メイン スレッドは Task.Run を呼び出したので、他の作業を実行できます...");

// Task の完了を待つ
task.Wait();

Console.WriteLine("Task の完了を確認しました。メイン プログラムを終了します。");
