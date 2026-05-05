using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("AsyncLocal<T> が実行コンテキストとともに流れるデモ");

await Demo.RunAsync();

static class Demo
{
    private static readonly AsyncLocal<string?> TraceId = new();

    public static async Task RunAsync()
    {
        TraceId.Value = "REQ-42";

        Console.WriteLine(
            $"await 前: trace = {TraceId.Value}, thread = {Environment.CurrentManagedThreadId}");

        await Task.Delay(100);

        Console.WriteLine(
            $"await 後: trace = {TraceId.Value}, thread = {Environment.CurrentManagedThreadId}");

        await Task.Run(() =>
        {
            Console.WriteLine(
                $"Task.Run の内側: trace = {TraceId.Value}, thread = {Environment.CurrentManagedThreadId}");
        });

        Task suppressedTask;
        using (ExecutionContext.SuppressFlow())
        {
            suppressedTask = Task.Run(() =>
            {
                Console.WriteLine(
                    $"フロー抑制中: trace = {TraceId.Value ?? "<null>"}, thread = {Environment.CurrentManagedThreadId}");
            });
        }

        await suppressedTask;
    }
}
