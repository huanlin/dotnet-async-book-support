// 示範 Interlocked 類別的原子操作
// 使用 Parallel.For 模擬多執行緒同時遞增計數器，並以 Interlocked.Increment 確保原子性。

int counter = 0;

Console.WriteLine("=== Interlocked.Increment 示範 ===");
Console.WriteLine("使用 Parallel.For 同時執行 1,000,000 次遞增操作...");

Parallel.For(0, 1_000_000, _ =>
{
    Interlocked.Increment(ref counter);
});

Console.WriteLine($"最終計數器值（應為 1000000）: {counter}");

// ---

Console.WriteLine("\n=== Interlocked.Add 示範 ===");
int total = 0;

Parallel.For(0, 10, i =>
{
    // 原子地加上 (i + 1) * 10
    Interlocked.Add(ref total, (i + 1) * 10);
});

// 預期結果：10 + 20 + 30 + ... + 100 = 550
Console.WriteLine($"加總結果（應為 550）: {total}");

// ---

Console.WriteLine("\n=== Interlocked.Exchange 示範 ===");
int status = 0; // 0 = 閒置，1 = 忙碌

// 設定為忙碌，並取得原本的值
int previous = Interlocked.Exchange(ref status, 1);
Console.WriteLine($"舊狀態: {previous}（0 = 閒置），新狀態: {status}（1 = 忙碌）");

// ---

Console.WriteLine("\n=== Interlocked.CompareExchange 示範 ===");
int value = 10;

// 若 value 等於 10，就將它換成 20；否則不做任何事
int original = Interlocked.CompareExchange(ref value, 20, 10);
Console.WriteLine($"CompareExchange 前: {original}，執行後: {value}（應為 20）");

// 再次執行：此時 value 是 20，不等於 10，所以不會交換
original = Interlocked.CompareExchange(ref value, 99, 10);
Console.WriteLine($"CompareExchange 前: {original}，執行後: {value}（應仍為 20）");
