using System.Threading;
using System.Threading.Tasks;

Console.WriteLine($"主執行緒 ID: {Thread.CurrentThread.ManagedThreadId}");

Console.WriteLine("準備使用 Task.Run 執行背景工作...");

// 將工作交給執行緒集區，並取得 Task 物件
Task task = Task.Run(() =>
{
    Console.WriteLine($"背景執行緒 ID: {Thread.CurrentThread.ManagedThreadId}");
    Console.WriteLine("背景工作正在進行中...");
    Thread.Sleep(2000); // 這裡只用來簡化模擬一段耗時的同步工作
    Console.WriteLine("背景工作完成。");
});

Console.WriteLine("主執行緒已呼叫 Task.Run，繼續執行其他事情...");

// 等待 Task 完成
task.Wait();

Console.WriteLine("確認 Task 已完成，主程式即將結束。");
