using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// 最大 3 つの操作だけが、この非同期作業へ同時に入れるようにする。
SemaphoreSlim _semaphore = new SemaphoreSlim(3, 3);

var tasks = new List<Task>();
for (int i = 1; i <= 10; i++)
{
    int taskId = i;
    tasks.Add(PerformExpensiveOperationAsync(taskId));
}

await Task.WhenAll(tasks);
Console.WriteLine("すべてのタスクが完了しました。");

async Task PerformExpensiveOperationAsync(int id)
{
    Console.WriteLine($"タスク {id} は入場待ちです...");
    // 空き枠を非同期に待つ。
    await _semaphore.WaitAsync();
    try
    {
        Console.WriteLine($"--> タスク {id} が入り、実行中です...");
        await Task.Delay(2000); // 重い作業をシミュレートする
    }
    finally
    {
        // 必ず空き枠を解放する。
        _semaphore.Release();
        Console.WriteLine($"<-- タスク {id} が出ました。");
    }
}
