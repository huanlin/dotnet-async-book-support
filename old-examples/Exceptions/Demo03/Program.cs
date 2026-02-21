
MyMethod();

Thread.Sleep(1500); // 確保其他執行緒來得及輸出訊息。

async Task MyMethod()
{
    try
    {
        File.ReadAllBytesAsync("d:\\Temp\\go.mod").ContinueWith(t =>
        {
            throw new NotImplementedException();
        });

        (new HttpClient()).GetStringAsync("https://microsoft.com").ContinueWith(t =>
        {
            throw new NotImplementedException();
        });
    }
    catch
    {
        Console.WriteLine("捕捉到異常!");
    }
}

// Output: 不會顯示「捕捉到異常!」 