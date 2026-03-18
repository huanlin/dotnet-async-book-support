using System.Collections.Immutable;

Console.WriteLine("示範第 6 章：ImmutableArray<T> 的 default 值陷阱");

ImmutableArray<int> items = default;

Console.WriteLine($"items.IsDefault = {items.IsDefault}");

try
{
    _ = items.Length;
}
catch (Exception ex)
{
    Console.WriteLine($"items.Length 拋出: {ex.GetType().Name}");
}

try
{
    _ = items.Add(42);
}
catch (Exception ex)
{
    Console.WriteLine($"items.Add(42) 拋出: {ex.GetType().Name}");
}

var safe = ImmutableArray<int>.Empty;
Console.WriteLine($"ImmutableArray<int>.Empty.Length = {safe.Length}");
