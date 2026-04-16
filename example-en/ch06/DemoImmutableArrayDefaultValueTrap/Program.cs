using System.Collections.Immutable;

Console.WriteLine("Demo: the default-value trap of ImmutableArray<T>");

ImmutableArray<int> items = default;

Console.WriteLine($"items.IsDefault = {items.IsDefault}");

try
{
    _ = items.Length;
}
catch (Exception ex)
{
    Console.WriteLine($"items.Length threw: {ex.GetType().Name}");
}

try
{
    _ = items.Add(42);
}
catch (Exception ex)
{
    Console.WriteLine($"items.Add(42) threw: {ex.GetType().Name}");
}

var safe = ImmutableArray<int>.Empty;
Console.WriteLine($"ImmutableArray<int>.Empty.Length = {safe.Length}");
