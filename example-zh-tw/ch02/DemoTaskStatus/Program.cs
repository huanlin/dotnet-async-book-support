// 示範 TaskStatus 列舉與快速狀態屬性

Console.WriteLine("建立並啟動一個會故意失敗的任務...");

Task myTask = Task.Run(() =>
{
    Console.WriteLine("任務開始執行，即將拋出例外...");
    Thread.Sleep(500);
    throw new InvalidOperationException("哎呀，任務失敗了！");
});                 

try
{
    // 故意等待任務完成，以觀察它的最終狀態
    myTask.Wait();
}
catch (AggregateException)
{
    // 使用 Wait() 等待失敗的 Task 時，例外會被包裹在 AggregateException 中。
    // 在此先捕獲並忽略，以觀察任務的最終狀態。
}

Console.WriteLine($"任務最終狀態: {myTask.Status}");
Console.WriteLine($"IsFaulted: {myTask.IsFaulted}");
Console.WriteLine($"IsCompleted: {myTask.IsCompleted}");
