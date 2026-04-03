using System;
using System.Threading;
using System.Threading.Tasks;

// 1. 建立 CancellationTokenSource
using var cts = new CancellationTokenSource();

// 2. 從 CTS 取得 CancellationToken
var token = cts.Token;

// 3. 將 token 傳遞給你的非同步方法 (先不要立刻 await，讓它在背景跑)
Task workTask = DoSomeLongRunningWorkAsync(token);

// 模擬使用者操作了一段時間後決定取消
await Task.Delay(2500);

// 4. 在未來的某個時間點，當你決定要取消時...
Console.WriteLine("\n[呼叫端] 使用者決定取消操作！");
cts.Cancel(); // 按下「取消按鈕」

try 
{
    await workTask; // 等待背景工作結束
} 
catch (OperationCanceledException) 
{
    // 這是預期的
    Console.WriteLine("呼叫端捕獲 OperationCanceledException。");
}
catch (Exception ex)
{
    // 這才是真正的錯誤
    Console.WriteLine($"工作發生異常：{ex.Message}");
}

static async Task DoSomeLongRunningWorkAsync(CancellationToken token)
{
    Console.WriteLine("背景工作已開始...");
    try
    {
        for (int i = 0; i < 10; i++)
        {
            // 檢查是否已經收到取消請求
            token.ThrowIfCancellationRequested();

            Console.WriteLine($"正在執行第 {i + 1}/10 部分的工作...");
            // 重要：將 token 繼續向下傳遞給任何支援的底層 API！
            await Task.Delay(1000, token);
        }
        Console.WriteLine("背景工作順利完成。");
    }
    catch (OperationCanceledException)
    {
        // ThrowIfCancellationRequested 會拋出 OperationCanceledException，
        // 而像 Task.Delay 這類 API 常會拋出其衍生型別 TaskCanceledException
        Console.WriteLine("背景工作已被取消。");
        throw; // 通常需要往上拋出
    }
}
