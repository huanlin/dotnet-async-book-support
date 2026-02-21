using System;
using System.Net.Http;
using System.Threading.Tasks;

// 使用 HttpClient 時，宣告為單一實例且可重複使用，避免 Socket 耗盡。
using var sharedClient = new HttpClient();

try
{
    string content = await DownloadPageAsync("https://this-url-does-not-exist.com");
    Console.WriteLine("下載成功！");
}
catch (HttpRequestException ex)
{
    Console.WriteLine("發生網路錯誤：");
    Console.WriteLine(ex.Message);
}
async Task<string> DownloadPageAsync(string url)
{
    // HttpClient 會在找不到網址時拋出 HttpRequestException
    string content = await sharedClient.GetStringAsync(url);
    return content;
}
