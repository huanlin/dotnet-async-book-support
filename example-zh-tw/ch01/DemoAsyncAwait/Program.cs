// 模式三：超級高效的智慧廚師 (Asynchronous Programming)
// 負責展示使用 async/await 釋放執行緒，達成非阻塞的等待

using System.Diagnostics;

Console.WriteLine("披薩店開始營業！模式三：超級廚師搭配智慧烤箱 (非同步程式設計)");
var sw = Stopwatch.StartNew();

// 啟動三個非同步的披薩製作任務（它們會併發執行非同步邏輯）
var p1 = MakePizzaAsync(1);
var p2 = MakePizzaAsync(2);
var p3 = MakePizzaAsync(3);

// 非同步等待所有披薩任務完成
await Task.WhenAll(p1, p2, p3);

sw.Stop();
Console.WriteLine($"所有披薩製作完成，總共耗時: {sw.ElapsedMilliseconds} 毫秒");

async Task MakePizzaAsync(int id)
{
    // 獲取目前執行緒的 ID，觀察非同步的執行緒變化
    int threadId = Environment.CurrentManagedThreadId;
    Console.WriteLine($"[廚師 {threadId}] 開始處理第 {id} 份披薩，先等待麵糰醒發...");

    // 模擬一段可非同步等待的前置時間，例如等待麵糰醒發或配料送達：不阻塞執行緒
    await Task.Delay(500);

    threadId = Environment.CurrentManagedThreadId;
    Console.WriteLine($"[廚師 {threadId}] 麵糰準備好了，將第 {id} 份披薩送入烤箱，設定計時器後即去處理其他事情！");

    // 模擬需要等待外部回應的耗時操作：烤箱烘烤。
    // await 期間，執行緒不會被阻塞，系統可以將該執行緒派去處理
    // 其他任務，直到烤箱時間到了再由系統安排繼續執行下一行程式。
    await Task.Delay(2000);

    threadId = Environment.CurrentManagedThreadId;
    Console.WriteLine($"[廚師 {threadId}] 「叮！」第 {id} 份披薩烤好了，廚師回來取出披薩。");
}
