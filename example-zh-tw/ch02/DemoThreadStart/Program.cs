using System.Threading;

var msg = $"主執行緒 ID: {Thread.CurrentThread.ManagedThreadId}";
Console.WriteLine(msg);

// 建立一條新的執行緒來執行 DoWork
var newThread = new Thread(DoWork);
newThread.Start();

Console.WriteLine("主執行緒繼續執行...");

void DoWork()
{
    var msg = $"背景執行緒 ID: {Thread.CurrentThread.ManagedThreadId}";
    Console.WriteLine(msg);
    Console.WriteLine("背景工作正在進行中...");
    Thread.Sleep(2000); // 模擬耗時 2 秒的工作
    Console.WriteLine("背景工作完成。");
}
