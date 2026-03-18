using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("示範 AsyncLocal<T> 隨 execution context 流動");

await Demo.RunAsync();

static class Demo
{
    private static readonly AsyncLocal<string?> TraceId = new();

    public static async Task RunAsync()
    {
        TraceId.Value = "REQ-42";

        Console.WriteLine(
            $"before await: trace = {TraceId.Value}, thread = {Environment.CurrentManagedThreadId}");

        await Task.Delay(100);

        Console.WriteLine(
            $"after await: trace = {TraceId.Value}, thread = {Environment.CurrentManagedThreadId}");

        await Task.Run(() =>
        {
            Console.WriteLine(
                $"inside Task.Run: trace = {TraceId.Value}, thread = {Environment.CurrentManagedThreadId}");
        });

        Task suppressedTask;
        using (ExecutionContext.SuppressFlow())
        {
            suppressedTask = Task.Run(() =>
            {
                Console.WriteLine(
                    $"suppressed flow: trace = {TraceId.Value ?? "<null>"}, thread = {Environment.CurrentManagedThreadId}");
            });
        }

        await suppressedTask;
    }
}
