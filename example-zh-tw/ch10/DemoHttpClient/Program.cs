using System.Diagnostics;

// 使用 HttpClient 時，宣告為單一實例且可重複使用，避免 Socket 耗盡
using var sharedClient = new HttpClient();

Console.WriteLine("示範 HttpClient 的 ResponseHeadersRead 與串流處理");

// 目標：下載一個大型檔案（例如 Ubuntu ISO，這裡使用公開可用的測速檔案取代或模擬）
// 為了避免真的下載太久，我們使用一個 100MB 的測試檔案
string url = "https://speed.hetzner.de/100MB.bin";
string tempFilePath = Path.GetTempFileName();

Console.WriteLine($"準備下載檔案: {url}");
Console.WriteLine($"預計儲存位置: {tempFilePath}");

try
{
    var sw = Stopwatch.StartNew();
    await DownloadLargeFileAsync(url, tempFilePath);
    sw.Stop();
    
    // 檢查檔案大小
    var fileInfo = new FileInfo(tempFilePath);
    Console.WriteLine($"\n✅ 下載完成！耗時: {sw.ElapsedMilliseconds} ms, 檔案大小: {fileInfo.Length / 1024 / 1024} MB");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ 發生錯誤: {ex.Message}");
}
finally
{
    // 清理暫存檔
    if (File.Exists(tempFilePath))
    {
        File.Delete(tempFilePath);
        Console.WriteLine("暫存檔已刪除。");
    }
}
async Task DownloadLargeFileAsync(string fileUrl, string destinationPath)
{
    // 關鍵參數：HttpCompletionOption.ResponseHeadersRead
    // 指示 HttpClient 只要讀到 HTTP Headers 就立刻返回，不要把整個 Body 讀進記憶體
    Console.WriteLine("發送 HTTP 要求，等待 Headers 回傳...");
    using var response = await sharedClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
    
    // 確保 HTTP 狀態碼是 2xx 成功
    response.EnsureSuccessStatusCode();

    // 取得即將進入的資料流總長度 (如果伺服器有提供 Content-Length)
    long? totalBytes = response.Content.Headers.ContentLength;
    Console.WriteLine($"伺服器回傳的檔案大小 (Content-Length): {totalBytes / 1024 / 1024} MB");
    
    Console.WriteLine("開始從網路流讀取資料，並同步寫入磁碟 (背壓控制與串流化處理)...");
    
    // 從 Response 中取得即時的非同步資料流
    using var networkStream = await response.Content.ReadAsStreamAsync();
    
    // 建立本地端的檔案流
    // ★ 關鍵點：必須設定 useAsync: true，才能讓作業系統底層真的使用非同步 I/O，不阻塞執行緒
    using var fileStream = new FileStream(
        destinationPath, 
        FileMode.Create, 
        FileAccess.Write, 
        FileShare.None, 
        bufferSize: 81920, 
        useAsync: true);

    // CopyToAsync 會在底層自動管理一塊固定大小的記憶體緩衝區
    // 當從網路讀取填滿緩衝區時，會將資料寫入磁碟。
    // 在寫入磁碟期間（如果磁碟較慢），網路讀取的動作會被暫停（背壓 backpressure），
    // 這確保了記憶體使用量不會因為網路快、磁碟慢而暴增。
    await networkStream.CopyToAsync(fileStream);
}
