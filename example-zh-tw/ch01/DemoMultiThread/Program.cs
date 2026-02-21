// 模式二：更靈活的多工廚師 (Multithreading)
// 負責展示多執行緒並行處理的現象

using System.Diagnostics;

Console.WriteLine("披薩店開始營業！模式二：有多位廚師同時工作 (多執行緒)");
var sw = Stopwatch.StartNew();

// 建立三個獨立的執行緒（分別代表三位不同的廚師）
Thread chef1 = new Thread(() => MakePizza(1));
Thread chef2 = new Thread(() => MakePizza(2));
Thread chef3 = new Thread(() => MakePizza(3));

// 讓所有廚師同時開始工作
chef1.Start();
chef2.Start();
chef3.Start();

// 主執行緒（餐廳經理）等待所有廚師完成工作
chef1.Join();
chef2.Join();
chef3.Join();

sw.Stop();
Console.WriteLine($"所有披薩製作完成，總共耗時: {sw.ElapsedMilliseconds} 毫秒");

void MakePizza(int id)
{
    int threadId = Environment.CurrentManagedThreadId;

    Console.WriteLine($"[廚師 {threadId}] 開始準備第 {id} 份披薩的餅皮...");
    Thread.Sleep(500); // 模擬切菜和揉麵的準備時間
    
    Console.WriteLine($"[廚師 {threadId}] 將第 {id} 份披薩送入烤箱，開始等待...");
    
    // Thread.Sleep 是阻塞操作，但因為每個披薩都有專屬的廚師（執行緒），
    // 某個廚師在等待時，其他的廚師仍然可以在自己的工作台上處理其他的披薩。
    Thread.Sleep(2000); 
    
    Console.WriteLine($"[廚師 {threadId}] 第 {id} 份披薩烤好了！取出披薩。");
}
