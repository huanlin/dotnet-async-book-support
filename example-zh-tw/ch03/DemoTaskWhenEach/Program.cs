using System;
using System.Collections.Generic;
using System.Threading.Tasks;

Console.WriteLine("示範 Task.WhenEach (.NET 9) 循序處理完成的工作");

await ProcessTasksAsTheyCompleteAsync();

// .NET 9+ 的新寫法：乾淨、直觀
async Task ProcessTasksAsTheyCompleteAsync()
{
    var tasks = new List<Task<int>>();
    for (int i = 1; i <= 5; i++)
    {
        tasks.Add(DoWorkAsync(i)); // 假設 DoWorkAsync 回傳 Task<int>
    }

    // 這裡的 t 代表一個「已經完成」的工作
    // 迴圈會依照「完成的順序」迭代，而不是原本串列的順序！
    await foreach (Task<int> t in Task.WhenEach(tasks))
    {
        try
        {
            int result = await t; // 這裡 await 只是為了取得結果或例外，不會阻塞
            Console.WriteLine($"完成了一個工作，結果是：{result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"有個工作失敗了：{ex.Message}");
        }
    }
}

// 模擬一個會隨機耗時的工作
async Task<int> DoWorkAsync(int id)
{
    int delay = new Random().Next(500, 2000);
    await Task.Delay(delay);
    return id * 10;
}
