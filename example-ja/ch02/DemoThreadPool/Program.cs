using System.Threading;

var msg = $"メイン スレッド ID: {Environment.CurrentManagedThreadId}";
Console.WriteLine(msg);

// 作業項目をスレッド プールへキューイングする
ThreadPool.QueueUserWorkItem(_ => DoWork());

Console.WriteLine("メイン スレッドは実行を続けます...");
Thread.Sleep(3000); // バックグラウンド作業の完了を待つ。そうしないと、メイン プログラムが先に終了する可能性がある

void DoWork()
{
    Console.WriteLine($"バックグラウンド スレッド ID: {Environment.CurrentManagedThreadId}");
    Console.WriteLine("バックグラウンド作業を実行中です...");
    Thread.Sleep(2000); // 2 秒分の作業をシミュレートする
    Console.WriteLine("バックグラウンド作業が完了しました。");
}
