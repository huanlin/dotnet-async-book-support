using System.Collections.Immutable;

Console.WriteLine("デモ: ImmutableArray<T> の default 値の罠");

ImmutableArray<int> items = default;

Console.WriteLine($"items.IsDefault = {items.IsDefault}");

try
{
    _ = items.Length;
}
catch (Exception ex)
{
    Console.WriteLine($"items.Length がスローしました: {ex.GetType().Name}");
}

try
{
    _ = items.Add(42);
}
catch (Exception ex)
{
    Console.WriteLine($"items.Add(42) がスローしました: {ex.GetType().Name}");
}

var safe = ImmutableArray<int>.Empty;
Console.WriteLine($"ImmutableArray<int>.Empty.Length = {safe.Length}");
