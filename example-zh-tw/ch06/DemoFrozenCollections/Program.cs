using System.Collections.Frozen;

Console.WriteLine("=== 示範凍結集合 (Frozen Collections) ===");

var source = new Dictionary<string, int>
{
    ["apple"] = 3,
    ["banana"] = 5
};

// 將一般的 Dictionary 轉換為凍結字典。
FrozenDictionary<string, int> frozen = source.ToFrozenDictionary();

// 後續修改原始 Dictionary，不會影響已建立好的 FrozenDictionary。
source["apple"] = 99;
source["orange"] = 7;

Console.WriteLine($"source[\"apple\"] = {source["apple"]}");
Console.WriteLine($"frozen[\"apple\"] = {frozen["apple"]}");
Console.WriteLine($"frozen.ContainsKey(\"orange\") = {frozen.ContainsKey("orange")}");
