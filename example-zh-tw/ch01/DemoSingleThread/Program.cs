// 模式一：同步的單一廚師 (Synchronous Single-Threading)
// 負責展示單執行緒阻塞的現象

using System.Diagnostics;

Console.WriteLine("披薩店開始營業！模式一：只有一位廚師 (單執行緒)");
var sw = Stopwatch.StartNew();

// 模擬依序製作三個披薩
MakePizza(1);
MakePizza(2);
MakePizza(3);

sw.Stop();
Console.WriteLine($"所有披薩製作完成，總共耗時: {sw.ElapsedMilliseconds} 毫秒");

void MakePizza(int id)
{
    Console.WriteLine($"[單一廚師] 開始準備第 {id} 份披薩的餅皮...");
    Thread.Sleep(500); // 模擬切菜和揉麵的準備時間
    
    Console.WriteLine($"[單一廚師] 將第 {id} 份披薩送入烤箱，開始等待...");
    
    // 這裡使用 Thread.Sleep 來模擬「阻塞操作 (blocking operation)」
    // 廚師在此期間什麼都不能做，只能死等烤箱完成，既無法準備下一份，也無法接聽電話
    Thread.Sleep(2000); 
    
    Console.WriteLine($"[單一廚師] 第 {id} 份披薩烤好了！取出披薩。");
}
