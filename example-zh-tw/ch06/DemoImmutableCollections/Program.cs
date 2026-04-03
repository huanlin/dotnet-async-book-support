using System.Collections.Immutable;

Console.WriteLine("=== 示範不可變集合 (Immutable Collections) ===");

// 建立一個初始的不可變串列
ImmutableList<int> list1 = ImmutableList.Create(1, 2);

// Add 操作會回傳一個新的串列，而不是修改原本的串列
ImmutableList<int> list2 = list1.Add(3);

Console.WriteLine($"list1: {string.Join(", ", list1)} (未改變)");
Console.WriteLine($"list2: {string.Join(", ", list2)}");
