using System.Diagnostics;

// 這個範例在整個程式執行期間只建立一次 HttpClient，避免短時間內頻繁建立與銷毀
using var httpClient = new HttpClient();

Console.WriteLine("示範 HttpClient 的 ResponseHeadersRead 與串流處理");

// 目標：下載一個較大的檔案（這裡使用公開可用的 10 MB 測試檔案，較不易受網路波動影響）
string url = "https://proof.ovh.net/files/10Mb.dat";
string tempFilePath = Path.GetTempFileName();

Console.WriteLine($"準備下載檔案: {url}");
Console.WriteLine($"預計儲存位置: {tempFilePath}");

try
{
    var sw = Stopwatch.StartNew();
    using var cancellationSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    await DownloadLargeFileAsync(url, tempFilePath, cancellationSource.Token);
    sw.Stop();
    
    // 檢查檔案大小
    var fileInfo = new FileInfo(tempFilePath);
    Console.WriteLine($"\n✅ 下載完成！耗時: {sw.ElapsedMilliseconds} ms, 檔案大小: {fileInfo.Length / 1024 / 1024} MB");
}
catch (OperationCanceledException)
{
    Console.WriteLine("❌ 下載逾時或被取消。若網路較慢，可調整 CancellationTokenSource 的時間。");
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
async Task DownloadLargeFileAsync(
    string fileUrl,
    string destinationPath,
    CancellationToken cancellationToken = default)
{
    // 關鍵參數：HttpCompletionOption.ResponseHeadersRead
    // 指示 HttpClient 只要讀到 HTTP Headers 就立刻返回，不要把整個 Body 讀進記憶體。
    // 後續 Body 的讀取則改由 CancellationToken 來控制取消或逾時。
    Console.WriteLine("發送 HTTP 要求，等待 Headers 回傳...");
    using var response = await httpClient.GetAsync(
        fileUrl,
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);
    
    // 確保 HTTP 狀態碼是 2xx 成功
    response.EnsureSuccessStatusCode();

    // 取得即將進入的資料流總長度 (如果伺服器有提供 Content-Length)
    long? totalBytes = response.Content.Headers.ContentLength;
    string contentLengthText = totalBytes.HasValue
        ? $"{totalBytes.Value / 1024 / 1024} MB"
        : "未提供";
    Console.WriteLine($"伺服器回傳的檔案大小 (Content-Length): {contentLengthText}");
    
    Console.WriteLine("開始從網路流讀取資料，並持續寫入磁碟 (背壓控制與串流化處理)...");
    
    // 從 Response 中取得即時的非同步資料流
    using var networkStream = await response.Content.ReadAsStreamAsync(cancellationToken);
    
    // 建立本地端的檔案流
    // 關鍵點：明確設定 useAsync: true，要求底層盡量採用非同步 I/O 路徑
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
    await networkStream.CopyToAsync(fileStream, 81920, cancellationToken);
}
