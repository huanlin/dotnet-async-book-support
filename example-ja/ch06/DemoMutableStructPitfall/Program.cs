Console.WriteLine("デモ: 可変 struct の落とし穴 (静かな失敗)");

// 1. 通常の配列の動作 (変更は成功する)
MutablePoint[] points = new MutablePoint[1];
points[0] = new MutablePoint { X = 10, Y = 20 };
points[0].Move(5, 5);  // 配列では、要素を直接変更できる。
Console.WriteLine($"配列内の点: X={points[0].X}, Y={points[0].Y}");
// 期待される出力: X=15, Y=25

// 2. List<T> の罠 (静かな失敗)
List<MutablePoint> list = new List<MutablePoint>
{
    new MutablePoint { X = 10, Y = 20 }
};

// フィールドを直接変更することは、コンパイラによりブロックされる (CS1612):
// list[0].X = 10; // エラー CS1612: 戻り値を変更できない...

// 本当に危険なのは、内部状態を変更するメソッド呼び出しである。
// コンパイラは許可するが、list[0] が返すのは一時的なコピーだけであり、
// そのため Move(5, 5) は実際の項目ではなく構造体のコピーを変更する。
// List に格納されている実体は変更されない。
list[0].Move(5, 5);

Console.WriteLine($"List 内の点: X={list[0].X}, Y={list[0].Y}");
// 実際の出力: X=10, Y=20 (何も変わっていない!)

Console.WriteLine(
    "\n結論: 実務では struct を readonly struct として設計することが強く推奨されるのは、このためです。");

Console.WriteLine(
    "\n--- 追加デモ: readonly メソッド内の防御的コピー ---");

var point = new Point { X = 3, Y = 4 };
point.PrintInfo();

// --- 型定義 ---

// これは悪い設計例: 変更可能な構造体。
public struct MutablePoint
{
    public int X;
    public int Y;

    // このメソッドは構造体自身を変更しようとする。
    public void Move(int dx, int dy)
    {
        X += dx;
        Y += dy;
    }
}

public struct Point
{
    public int X;
    public int Y;

    public readonly void PrintInfo()
    {
        Console.WriteLine($"点: ({X}, {Y})");
        LogState();  // CS8656 と防御的コピーを観察するため、意図的に非 readonly にしている。
    }

    private void LogState()
    {
        Console.WriteLine($"現在の状態: X={X}, Y={Y}");
    }
}
