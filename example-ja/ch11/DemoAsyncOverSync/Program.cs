using System.Diagnostics;

Console.WriteLine("Async-over-Sync のトレードオフのデモ");

const int max = 50_000;

var badService = new BadPrimeService();
var goodService = new GoodPrimeService();

Console.WriteLine("\n--- 問題の兆候: ライブラリが自分で Task.Run を包む ---");
var sw = Stopwatch.StartNew();
int badCount = await badService.CountPrimesAsync(max);
sw.Stop();
Console.WriteLine($"呼び出し元が結果を受け取りました: {badCount}, 経過時間: {sw.ElapsedMilliseconds} ms");

Console.WriteLine("\n--- よりよい方法: ライブラリは同期のままにし、Task.Run を使うかどうかは呼び出し元が決める ---");
sw.Restart();
int goodCount = await Task.Run(() => goodService.CountPrimes(max));
sw.Stop();
Console.WriteLine($"呼び出し元が結果を受け取りました: {goodCount}, 経過時間: {sw.ElapsedMilliseconds} ms");

public sealed class BadPrimeService
{
    public Task<int> CountPrimesAsync(int max)
    {
        Console.WriteLine($"[BadService] 呼び出し元スレッド: {Environment.CurrentManagedThreadId}");

        return Task.Run(() =>
        {
            Console.WriteLine($"[BadService] ライブラリが一方的に CPU 作業をスレッド {Environment.CurrentManagedThreadId} へ移しました");
            return PrimeCounter.CountPrimes(max);
        });
    }
}

public sealed class GoodPrimeService
{
    public int CountPrimes(int max)
    {
        Console.WriteLine($"[GoodService] CPU 作業は実際にスレッド {Environment.CurrentManagedThreadId} で実行されています");
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
