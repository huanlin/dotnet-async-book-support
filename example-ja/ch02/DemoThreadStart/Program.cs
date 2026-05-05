using System.Threading;

var msg = $"メイン スレッド ID: {Environment.CurrentManagedThreadId}";
Console.WriteLine(msg);

// DoWork を実行する新しいスレッドを作成する
var newThread = new Thread(DoWork);
newThread.Start();

Console.WriteLine("メイン スレッドは実行を続けます...");

void DoWork()
{
    var msg = $"ワーカー スレッド ID: {Environment.CurrentManagedThreadId}";
    Console.WriteLine(msg);
    Console.WriteLine("作業中です...");
    Thread.Sleep(2000); // 2 秒分の作業をシミュレートする
    Console.WriteLine("作業が完了しました。");
}
