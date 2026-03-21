Console.WriteLine("示範第 6 章：不可變的 record 型別與 with 運算式");

// 1. 初始化一個 record 實體
var alice = new Person("Alice", 30);
Console.WriteLine($"原始物件: {alice}");
// 輸出會自動格式化為：Person { Name = Alice, Age = 30 }

// 2. 嘗試修改屬性會導致編譯錯誤
// alice.Age = 31; // 錯誤 CS8852: Init-only property or indexer 'Person.Age' can only be assigned in an object initializer...

// 3. 展示值相等性 (Value Equality)
var aliceClone = new Person("Alice", 30);
Console.WriteLine($"alice == aliceClone ? {alice == aliceClone}"); 
// 輸出 True！即使它們是記憶體中兩個不同的實體。

// 4. 展示非破壞性修改: with 運算式
var olderAlice = alice with { Age = 31 };
Console.WriteLine($"\n使用 with 運算式建立新版本:");
Console.WriteLine($"舊版本 (完全沒變): {alice}");
Console.WriteLine($"新版本: {olderAlice}");

// --- record 定義 ---

// 這短短一行程式碼，編譯器會在背後為我們產生:
// 1. Name 和 Age 兩個 init-only 屬性
// 2. 覆寫 Equals 和 GetHashCode 實現「值相等性」
// 3. 提供漂亮的 ToString() 實作
// 4. 支援 with 運算式所需的複製語意與解構 (Deconstruct) 語法
public record Person(string Name, int Age);
