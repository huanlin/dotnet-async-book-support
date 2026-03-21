using System.Threading;

var msg = $"主執行緒 ID: {Environment.CurrentManagedThreadId}";
Console.WriteLine(msg);

// 將工作項目排入執行緒集區
ThreadPool.QueueUserWorkItem(_ => DoWork());

Console.WriteLine("主執行緒繼續執行...");
Thread.Sleep(3000); // 等待背景工作完成，否則主程式可能先結束

void DoWork()
{
    Console.WriteLine($"背景執行緒 ID: {Environment.CurrentManagedThreadId}");
    Console.WriteLine("背景工作正在進行中...");
    Thread.Sleep(2000); // 模擬耗時 2 秒的工作
    Console.WriteLine("背景工作完成。");
}
