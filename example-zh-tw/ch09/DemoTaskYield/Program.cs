using System.Threading.Tasks;

Console.WriteLine("示範 Task.Yield() 建立非同步切點");
Console.WriteLine($"Main thread id: {Environment.CurrentManagedThreadId}");

Console.WriteLine("1. 呼叫端：呼叫 DemoAsync()");
Task task = DemoAsync();
Console.WriteLine("2. 呼叫端：DemoAsync() 已回傳 Task");

await task;
Console.WriteLine("5. 呼叫端：Task 已完成");

static async Task DemoAsync()
{
    Console.WriteLine($"3. 方法內：在 await Task.Yield() 之前，thread id = {Environment.CurrentManagedThreadId}");

    // 強制在這裡切出一個 async boundary，讓呼叫端先繼續執行。
    await Task.Yield();

    Console.WriteLine($"4. 方法內：在 await Task.Yield() 之後，thread id = {Environment.CurrentManagedThreadId}");
}
