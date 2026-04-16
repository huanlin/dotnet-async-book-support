Console.WriteLine("Demo: the pitfalls of a mutable struct (silent failure)");

// 1. Normal array behavior (mutation succeeds)
MutablePoint[] points = new MutablePoint[1];
points[0] = new MutablePoint { X = 10, Y = 20 };
points[0].Move(5, 5);  // For arrays, mutating the element directly works.
Console.WriteLine($"Point in array: X={points[0].X}, Y={points[0].Y}");
// Expected output: X=15, Y=25

// 2. The trap with List<T> (silent failure)
List<MutablePoint> list = new List<MutablePoint>
{
    new MutablePoint { X = 10, Y = 20 }
};

// Directly mutating a field is blocked by the compiler (CS1612):
// list[0].X = 10; // Error CS1612: Cannot modify the return value...

// The truly dangerous part: calling a method that mutates internal state.
// The compiler allows it, but list[0] returns only a temporary copy
// of the struct, so Move(5, 5) mutates the copy instead of the actual item
// stored in the List.
list[0].Move(5, 5);

Console.WriteLine($"Point in List: X={list[0].X}, Y={list[0].Y}");
// Actual output: X=10, Y=20 (nothing changed!)

Console.WriteLine(
    "\nConclusion: this is why it is strongly recommended in practice to design a struct as readonly struct.");

Console.WriteLine(
    "\n--- Extra demo: defensive copy inside a readonly method ---");

var point = new Point { X = 3, Y = 4 };
point.PrintInfo();

// --- Type definitions ---

// This is a bad design example: a mutable struct.
public struct MutablePoint
{
    public int X;
    public int Y;

    // This method attempts to mutate the struct itself.
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
        LogState();  // Intentionally non-readonly to observe CS8656 and the defensive copy.
    }

    private void LogState()
    {
        Console.WriteLine($"Current state: X={X}, Y={Y}");
    }
}
