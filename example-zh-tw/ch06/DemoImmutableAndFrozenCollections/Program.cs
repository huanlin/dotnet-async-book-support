using System.Collections.Frozen;
using System.Collections.Immutable;

Console.WriteLine("示範第 6 章：ImmutableList 與 FrozenDictionary");

Console.WriteLine("\n=== ImmutableList<T>：每次修改都產生新版本 ===");
ImmutableList<string> list1 = ImmutableList.Create("A", "B");
ImmutableList<string> list2 = list1.Add("C");

Console.WriteLine($"原始清單: {string.Join(", ", list1)}");
Console.WriteLine($"新清單:   {string.Join(", ", list2)}");

Console.WriteLine("\n=== FrozenDictionary<TKey, TValue>：建立一次，之後高速唯讀查詢 ===");
var source = new Dictionary<string, int>
{
    ["apple"] = 3,
    ["banana"] = 5
};

FrozenDictionary<string, int> frozen = source.ToFrozenDictionary();

// 後續修改原始 Dictionary，不會影響已建立好的 FrozenDictionary。
source["apple"] = 99;
source["orange"] = 7;

Console.WriteLine($"source[\"apple\"] = {source["apple"]}");
Console.WriteLine($"frozen[\"apple\"] = {frozen["apple"]}");
Console.WriteLine($"frozen.ContainsKey(\"orange\") = {frozen.ContainsKey("orange")}");
