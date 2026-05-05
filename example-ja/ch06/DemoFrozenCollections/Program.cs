using System.Collections.Frozen;

Console.WriteLine("=== Frozen Collections のデモ ===");

var source = new Dictionary<string, int>
{
    ["apple"] = 3,
    ["banana"] = 5
};

// 通常の Dictionary を frozen dictionary に変換する。
FrozenDictionary<string, int> frozen = source.ToFrozenDictionary();

// その後で元の Dictionary を変更しても、
// すでに作成済みの FrozenDictionary には影響しない。
source["apple"] = 99;
source["orange"] = 7;

Console.WriteLine($"source[\"apple\"] = {source["apple"]}");
Console.WriteLine($"frozen[\"apple\"] = {frozen["apple"]}");
Console.WriteLine(
    $"frozen.ContainsKey(\"orange\") = {frozen.ContainsKey("orange")}");
