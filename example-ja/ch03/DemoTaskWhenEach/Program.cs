using System;
using System.Collections.Generic;
using System.Threading.Tasks;

Console.WriteLine("Task.WhenEach (.NET 9) で完了した順にタスクを処理するデモ");

await ProcessTasksAsTheyCompleteAsync();

// .NET 9 以降の新機能: 簡潔で直感的
async Task ProcessTasksAsTheyCompleteAsync()
{
    var tasks = new List<Task<int>>();
    for (int i = 1; i <= 5; i++)
    {
        tasks.Add(DoWorkAsync(i)); // DoWorkAsync は Task<int> を返すと仮定する
    }

    // ここで t は、すでに完了したタスクを表す
    // ループは元のリスト順ではなく、完了順に反復する。
    await foreach (Task<int> t in Task.WhenEach(tasks))
    {
        try
        {
            int result = await t; // ここでの await は結果または例外を取り出すためだけのもので、ブロックはしない。
            Console.WriteLine($"1 つのタスクが完了しました。結果: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"1 つのタスクが失敗しました: {ex.Message}");
        }
    }
}

// ランダムな時間がかかる作業をシミュレートする
async Task<int> DoWorkAsync(int id)
{
    int delay = new Random().Next(500, 2000);
    await Task.Delay(delay);
    return id * 10;
}
