using System.Collections.Immutable;

Console.WriteLine("=== Immutable Collections のデモ ===");

// 初期のイミュータブル リストを作成する。
ImmutableList<int> list1 = ImmutableList.Create(1, 2);

// Add は元のリストを変更せず、新しいリストを返す。
ImmutableList<int> list2 = list1.Add(3);

Console.WriteLine($"list1: {string.Join(", ", list1)} (変更なし)");
Console.WriteLine($"list2: {string.Join(", ", list2)}");
