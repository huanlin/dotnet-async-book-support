using System;
using System.Threading.Tasks;

int counter = 0;
// 建立一個私有的鎖物件
object _lock = new object();

Task task1 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        // 進入臨界區段
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
        lock (_lock)
        {
            counter++;
        }
    }
});

Task.WaitAll(task1, task2);
Console.WriteLine($"計數器的最終結果是: {counter}"); // 結果永遠是 2000000
