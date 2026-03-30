using System;
using System.Net.Http;
using System.Threading.Tasks;

// 使用 HttpClient 時，宣告為單一實例且可重複使用，避免 Socket 耗盡
using var httpClient = new HttpClient();

try
{
    string url = "https://this-host-does-not-exist.invalid";
    string content = await DownloadPageAsync(url);
    Console.WriteLine("下載成功！");
}
catch (HttpRequestException ex)
{
    Console.WriteLine("發生網路錯誤：");
    Console.WriteLine(ex.Message);
}

async Task<string> DownloadPageAsync(string url)
{
    // HttpClient 在要求失敗或無法解析主機名稱時，可能拋出 HttpRequestException
    string content = await httpClient.GetStringAsync(url);
    return content;
}
