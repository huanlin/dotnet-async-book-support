Console.WriteLine("示範可變 struct 的陷阱 (靜默失敗)");

// 1. 正常的陣列行為 (成功修改)
MutablePoint[] points = new MutablePoint[1];
points[0] = new MutablePoint { X = 10, Y = 20 };
points[0].Move(5, 5);  // 對於陣列，直接修改元素是有效的
Console.WriteLine($"陣列中的點：X={points[0].X}, Y={points[0].Y}");
// 期待輸出: X=15, Y=25


// 2. 泛型 List<T> 的陷阱行為 (靜默失敗)
List<MutablePoint> list = new List<MutablePoint>
{
    new MutablePoint { X = 10, Y = 20 }
};

// 直接修改欄位會被編譯器擋下（CS1612）：
// list[0].X = 10; // 錯誤 CS1612: 無法修改 'List<MutablePoint>.this[int]' 的傳回值，因為它不是變數

// 真正危險的地方：呼叫會修改內部狀態的方法，編譯器不會報錯。
// list[0] 會回傳該結構的一個「暫時副本 (Copy)」，
// Move(5, 5) 只修改了那個副本，並不會寫回 List 內部的元素。
list[0].Move(5, 5);

Console.WriteLine($"List 中的點：X={list[0].X}, Y={list[0].Y}");
// 實際輸出: X=10, Y=20 (完全沒變！)

Console.WriteLine("\n結論：這就是為何實務上強烈建議將 struct 設計為 readonly struct 的原因。");

Console.WriteLine("\n--- 額外示範：readonly 方法中的 defensive copy ---");

var point = new Point { X = 3, Y = 4 };
point.PrintInfo();


// --- 結構定義 ---

// 這是一個不良設計的示範：可變的 struct (Mutable struct)
public struct MutablePoint
{
    public int X;
    public int Y;

    // 這個方法試圖改變 struct 自身的狀態
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
        Console.WriteLine($"Point: ({X}, {Y})");
        LogState();  // 刻意保留非 readonly，以便觀察 CS8656 警告與 defensive copy
    }

    private void LogState()
    {
        Console.WriteLine($"Current state: X={X}, Y={Y}");
    }
}
