Console.WriteLine("デモ: 不変 record 型と with 式");

// 1. レコード インスタンスを作成する。
var alice = new Person("Alice", 30);
Console.WriteLine($"元のオブジェクト: {alice}");
// 出力は自動的に次の形式になる:
// Person { Name = Alice, Age = 30 }

// 2. プロパティを変更しようとするとコンパイル エラーになる。
// alice.Age = 31; // エラー CS8852: init-only プロパティ...

// 3. 値の等価性を示す。
var aliceClone = new Person("Alice", 30);
Console.WriteLine($"alice == aliceClone ? {alice == aliceClone}");
// メモリ上では別々のオブジェクトだが、True と表示される。

// 4. with 式による非破壊的な変更を示す。
var olderAlice = alice with { Age = 31 };
Console.WriteLine("\nwith 式で新しいバージョンを作成します:");
Console.WriteLine($"古いバージョン (まったく変更なし): {alice}");
Console.WriteLine($"新しいバージョン: {olderAlice}");

// --- レコード定義 ---

// この短い 1 行だけで、コンパイラは次のものを生成する:
// 1. 2 つの init-only プロパティ: Name と Age
// 2. 値の等価性のための Equals と GetHashCode
// 3. 読みやすい ToString() 実装
// 4. with 式に必要なコピー セマンティクス
// 5. `var (name, age) = person` のための Deconstruct メソッド
public record Person(string Name, int Age);
