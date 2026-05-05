using System;
using System.Threading.Tasks;

int counter = 0;

// counter を並行してインクリメントする 2 つのタスクを開始する。
Task task1 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        counter++;
    }
});

Task task2 = Task.Run(() =>
{
    for (int i = 0; i < 1_000_000; i++)
    {
        counter++;
    }
});

// 両方のタスクが終わるのを待つ。
Task.WaitAll(task1, task2);

Console.WriteLine($"最終的なカウンター値: {counter}");
