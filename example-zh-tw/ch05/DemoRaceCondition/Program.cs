using System;
using System.Threading.Tasks;

int counter = 0;

// 啟動兩個 Task，併發地對 counter 進行遞增
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

// 等待兩個 Task 都完成
Task.WaitAll(task1, task2);

Console.WriteLine($"計數器的最終結果是: {counter}");
