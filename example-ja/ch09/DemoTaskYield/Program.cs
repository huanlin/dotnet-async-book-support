using System.Threading.Tasks;

Console.WriteLine("Task.Yield() が非同期境界を作るデモ");
Console.WriteLine($"メイン スレッド ID: {Environment.CurrentManagedThreadId}");

Console.WriteLine("1. 呼び出し元: DemoAsync() を呼び出します");
Task task = DemoAsync();
Console.WriteLine("2. 呼び出し元: DemoAsync() が Task を返しました");

await task;
Console.WriteLine("5. 呼び出し元: Task が完了しました");

static async Task DemoAsync()
{
    Console.WriteLine($"3. メソッド内: await Task.Yield() の前, スレッド ID = {Environment.CurrentManagedThreadId}");

    // 呼び出し元が先に続行できるように、ここで非同期境界を作る。
    await Task.Yield();

    Console.WriteLine($"4. メソッド内: await Task.Yield() の後, スレッド ID = {Environment.CurrentManagedThreadId}");
}
