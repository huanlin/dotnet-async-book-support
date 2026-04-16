using System.Collections.Immutable;

Console.WriteLine("=== Immutable Collections demo ===");

// Create an initial immutable list.
ImmutableList<int> list1 = ImmutableList.Create(1, 2);

// Add returns a new list instead of modifying the original list.
ImmutableList<int> list2 = list1.Add(3);

Console.WriteLine($"list1: {string.Join(", ", list1)} (unchanged)");
Console.WriteLine($"list2: {string.Join(", ", list2)}");
