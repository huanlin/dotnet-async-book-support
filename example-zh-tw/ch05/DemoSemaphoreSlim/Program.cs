using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// 限制最多只能有 3 個操作同時進入這段非同步工作
SemaphoreSlim _semaphore = new SemaphoreSlim(3);

var tasks = new List<Task>();
for (int i = 1; i <= 10; i++)
{
    int taskId = i;
    tasks.Add(PerformExpensiveOperationAsync(taskId));
}

await Task.WhenAll(tasks);
Console.WriteLine("所有任務執行完畢。");

async Task PerformExpensiveOperationAsync(int id)
{
    Console.WriteLine($"任務 {id} 正在等待進入...");
    // 非同步地等待號誌（手環）
    await _semaphore.WaitAsync();
    try
    {
        Console.WriteLine($"--> 任務 {id} 已進入，正在執行...");
        await Task.Delay(2000); // 模擬耗時工作
    }
    finally
    {
        // 確保一定會釋放號誌（手環）
        _semaphore.Release();
        Console.WriteLine($"<-- 任務 {id} 已離開。");
    }
}
