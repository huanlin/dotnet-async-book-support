using System.Net.Http;
using System.Threading.Tasks;

// 使用 HttpClient 時，宣告為單一實例且可重複使用，避免 Socket 耗盡
using var httpClient = new HttpClient();

Console.WriteLine("使用 async/await 下載網頁並計算字數");

string url = "https://example.com";
int count = await DownloadPageAndCountCharsAsync(url);

Console.WriteLine($"網址 {url} 的字數是: {count}");

async Task<int> DownloadPageAndCountCharsAsync(string url)
{
    // 1. 非同步地等待下載完成，期間不會阻塞執行緒
    string content = await httpClient.GetStringAsync(url);
    // 2. 下載完成後，程式碼會從這裡繼續執行
    return content.Length;
}
