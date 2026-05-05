using System;
using System.Net.Http;
using System.Threading.Tasks;

// ソケット枯渇を避けるため、HttpClient インスタンスを 1 つだけ再利用する。
using var httpClient = new HttpClient();

try
{
    string url = "https://this-host-does-not-exist.invalid";
    string content = await DownloadPageAsync(url);
    Console.WriteLine("ダウンロードに成功しました!");
}
catch (HttpRequestException ex)
{
    Console.WriteLine("ネットワーク エラーが発生しました:");
    Console.WriteLine(ex.Message);
}

async Task<string> DownloadPageAsync(string url)
{
    // リクエストが失敗した場合、HttpClient は HttpRequestException をスローする可能性がある
    // またはホスト名を解決できない場合にもスローする可能性がある。
    string content = await httpClient.GetStringAsync(url);
    return content;
}
