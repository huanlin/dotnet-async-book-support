// Demonstrate atomic operations provided by Interlocked.
// Use Parallel.For to simulate many threads incrementing a counter at once,
// and use Interlocked.Increment to guarantee atomicity.

int counter = 0;

Console.WriteLine("=== Interlocked.Increment demo ===");
Console.WriteLine("Running 1,000,000 increments concurrently with Parallel.For...");

Parallel.For(0, 1_000_000, _ =>
{
    Interlocked.Increment(ref counter);
});

Console.WriteLine($"Final counter value (should be 1000000): {counter}");

// ---

Console.WriteLine("\n=== Interlocked.Add demo ===");
int total = 0;

Parallel.For(0, 10, i =>
{
    // Add (i + 1) * 10 atomically.
    Interlocked.Add(ref total, (i + 1) * 10);
});

// Expected result: 10 + 20 + 30 + ... + 100 = 550
Console.WriteLine($"Sum result (should be 550): {total}");

// ---

Console.WriteLine("\n=== Interlocked.Exchange demo ===");
int status = 0; // 0 = idle, 1 = busy

// Set the status to busy and retrieve the previous value.
int previous = Interlocked.Exchange(ref status, 1);
Console.WriteLine(
    $"Previous status: {previous} (0 = idle), new status: {status} (1 = busy)");

// ---

Console.WriteLine("\n=== Interlocked.CompareExchange demo ===");
int value = 10;

// If value equals 10, replace it with 20. Otherwise, do nothing.
int original = Interlocked.CompareExchange(ref value, 20, 10);
Console.WriteLine(
    $"Value before CompareExchange: {original}, after execution: {value} (should be 20)");

// Run it again: value is now 20, so it no longer equals 10,
// and the exchange does not happen.
original = Interlocked.CompareExchange(ref value, 99, 10);
Console.WriteLine(
    $"Value before CompareExchange: {original}, after execution: {value} (should still be 20)");
