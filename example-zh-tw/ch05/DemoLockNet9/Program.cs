using System;
using System.Threading;
using System.Threading.Tasks;

int counter = 0;
// 1. 使用專用的 Lock 型別，而不是 object
Lock _lock = new Lock();

Task task1 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        // 2. 這語法看起來跟舊的一樣，但編譯器會針對 Lock 型別產生更高效的程式碼！
        lock (_lock)
        {
            counter++;
        }
    }
});

Task task2 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        // 使用 EnterScope 的寫法
        using (_lock.EnterScope())
        {
            // 進入關鍵區段
            counter++;
        } // 離開區塊時自動釋放鎖
    }
});

Task.WaitAll(task1, task2);
Console.WriteLine($"計數器的最終結果是: {counter}");
