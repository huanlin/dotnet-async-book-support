using System.Collections.Frozen;

Console.WriteLine("=== Frozen Collections demo ===");

var source = new Dictionary<string, int>
{
    ["apple"] = 3,
    ["banana"] = 5
};

// Convert a normal Dictionary into a frozen dictionary.
FrozenDictionary<string, int> frozen = source.ToFrozenDictionary();

// Later changes to the original Dictionary do not affect the
// FrozenDictionary that was already created.
source["apple"] = 99;
source["orange"] = 7;

Console.WriteLine($"source[\"apple\"] = {source["apple"]}");
Console.WriteLine($"frozen[\"apple\"] = {frozen["apple"]}");
Console.WriteLine(
    $"frozen.ContainsKey(\"orange\") = {frozen.ContainsKey("orange")}");
