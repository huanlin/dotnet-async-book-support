using System.Diagnostics;

Console.WriteLine("示範 Async-over-Sync 的取捨");

const int max = 50_000;

var badService = new BadPrimeService();
var goodService = new GoodPrimeService();

Console.WriteLine("\n--- 壞味道：Library 自己包 Task.Run ---");
var sw = Stopwatch.StartNew();
int badCount = await badService.CountPrimesAsync(max);
sw.Stop();
Console.WriteLine($"呼叫端拿到結果：{badCount}，耗時 {sw.ElapsedMilliseconds} ms");

Console.WriteLine("\n--- 較佳做法：Library 保持同步，由呼叫端決定是否 Task.Run ---");
sw.Restart();
int goodCount = await Task.Run(() => goodService.CountPrimes(max));
sw.Stop();
Console.WriteLine($"呼叫端拿到結果：{goodCount}，耗時 {sw.ElapsedMilliseconds} ms");

public sealed class BadPrimeService
{
    public Task<int> CountPrimesAsync(int max)
    {
        Console.WriteLine($"[BadService] 呼叫端執行緒：{Environment.CurrentManagedThreadId}");

        return Task.Run(() =>
        {
            Console.WriteLine($"[BadService] Library 擅自把 CPU 工作丟到執行緒 {Environment.CurrentManagedThreadId}");
            return PrimeCounter.CountPrimes(max);
        });
    }
}

public sealed class GoodPrimeService
{
    public int CountPrimes(int max)
    {
        Console.WriteLine($"[GoodService] CPU 工作實際執行於執行緒 {Environment.CurrentManagedThreadId}");
        return PrimeCounter.CountPrimes(max);
    }
}

public static class PrimeCounter
{
    public static int CountPrimes(int max)
    {
        var count = 0;

        for (int candidate = 2; candidate <= max; candidate++)
        {
            if (IsPrime(candidate))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsPrime(int number)
    {
        for (int divisor = 2; divisor * divisor <= number; divisor++)
        {
            if (number % divisor == 0)
            {
                return false;
            }
        }

        return number > 1;
    }
}
