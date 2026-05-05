// Interlocked が提供するアトミック操作を示す。
// Parallel.For で多数のスレッドが同時にカウンターを増やす状況をシミュレートし、
// Interlocked.Increment でアトミック性を保証する。

int counter = 0;

Console.WriteLine("=== Interlocked.Increment のデモ ===");
Console.WriteLine("Parallel.For で 1,000,000 回のインクリメントを並行実行します...");

Parallel.For(0, 1_000_000, _ =>
{
    Interlocked.Increment(ref counter);
});

Console.WriteLine($"最終的なカウンター値 (1000000 になるはず): {counter}");

// ---

Console.WriteLine("\n=== Interlocked.Add のデモ ===");
int total = 0;

Parallel.For(0, 10, i =>
{
    // (i + 1) * 10 をアトミックに加算する。
    Interlocked.Add(ref total, (i + 1) * 10);
});

// 期待される結果: 10 + 20 + 30 + ... + 100 = 550
Console.WriteLine($"合計結果 (550 になるはず): {total}");

// ---

Console.WriteLine("\n=== Interlocked.Exchange のデモ ===");
int status = 0; // 0 = アイドル、1 = ビジー

// 状態をビジーに設定し、直前の値を取得する。
int previous = Interlocked.Exchange(ref status, 1);
Console.WriteLine(
    $"以前の状態: {previous} (0 = 待機中), 新しい状態: {status} (1 = 処理中)");

// ---

Console.WriteLine("\n=== Interlocked.CompareExchange のデモ ===");
int value = 10;

// value が 10 なら 20 に置き換える。それ以外なら何もしない。
int original = Interlocked.CompareExchange(ref value, 20, 10);
Console.WriteLine(
    $"CompareExchange 前の値: {original}, 実行後: {value} (20 になるはず)");

// もう一度実行する。value はすでに 20 なので 10 とは一致せず、
// 交換は行われない。
original = Interlocked.CompareExchange(ref value, 99, 10);
Console.WriteLine(
    $"CompareExchange 前の値: {original}, 実行後: {value} (まだ 20 のはず)");
