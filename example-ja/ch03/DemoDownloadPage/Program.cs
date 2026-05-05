using System.Net.Http;
using System.Threading.Tasks;

// ソケット枯渇を避けるため、HttpClient インスタンスを 1 つだけ再利用する
using var httpClient = new HttpClient();

Console.WriteLine("async/await を使って Web ページをダウンロードし、文字数を数えます");

string url = "https://example.com";
int count = await DownloadPageAndCountCharsAsync(url);

Console.WriteLine($"{url} の文字数: {count}");

async Task<int> DownloadPageAndCountCharsAsync(string url)
{
    // 1. スレッドをブロックせずに、ダウンロードの完了を非同期に待つ
    string content = await httpClient.GetStringAsync(url);
    // 2. ダウンロードが完了すると、ここから実行が続く
    return content.Length;
}
