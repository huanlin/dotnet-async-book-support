using System.Threading;
using System.Threading.Tasks;

// 範例一：使用 Thread (前景執行緒)
var foregroundThread = new Thread(() =>
{
    Thread.Sleep(3000);
    Console.WriteLine("前景執行緒完成。");
});
// foregroundThread.IsBackground = true; // 可以手動設為背景
foregroundThread.Start();
Console.WriteLine("Main 函式 (前景) 即將結束，但程式會等待前景執行緒完成。");


// 範例二：使用 Task.Run (背景執行緒)
_ = Task.Run(() =>
{
    Thread.Sleep(5000);
    // 這行可能永遠不會被執行
    Console.WriteLine("背景執行緒完成。");
});
Console.WriteLine("Main 函式 (前景) 即將結束，程式不會等待背景執行緒。");
