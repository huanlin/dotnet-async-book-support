using System.Diagnostics;

Console.WriteLine("Demonstrating the tradeoff of Async-over-Sync");

const int max = 50_000;

var badService = new BadPrimeService();
var goodService = new GoodPrimeService();

Console.WriteLine("\n--- Smell: the library wraps Task.Run by itself ---");
var sw = Stopwatch.StartNew();
int badCount = await badService.CountPrimesAsync(max);
sw.Stop();
Console.WriteLine($"The caller received the result: {badCount}, elapsed time: {sw.ElapsedMilliseconds} ms");

Console.WriteLine("\n--- Better approach: the library stays synchronous, and the caller decides whether to use Task.Run ---");
sw.Restart();
int goodCount = await Task.Run(() => goodService.CountPrimes(max));
sw.Stop();
Console.WriteLine($"The caller received the result: {goodCount}, elapsed time: {sw.ElapsedMilliseconds} ms");

public sealed class BadPrimeService
{
    public Task<int> CountPrimesAsync(int max)
    {
        Console.WriteLine($"[BadService] Caller thread: {Environment.CurrentManagedThreadId}");

        return Task.Run(() =>
        {
            Console.WriteLine($"[BadService] The library unilaterally moved CPU work onto thread {Environment.CurrentManagedThreadId}");
            return PrimeCounter.CountPrimes(max);
        });
    }
}

public sealed class GoodPrimeService
{
    public int CountPrimes(int max)
    {
        Console.WriteLine($"[GoodService] The CPU work is actually running on thread {Environment.CurrentManagedThreadId}");
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
