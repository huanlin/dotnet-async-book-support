
try
{
    await MyMethod();
}
catch (Exception ex)
{
    Console.WriteLine("捕捉到異常! " + ex.GetType().FullName);
}


async Task MyMethod()
{
    await new HttpClient().GetStringAsync("https://microsoft.com");
    await File.ReadAllBytesAsync("file.txt");
    throw new NotImplementedException();
}

// Output: 捕捉到異常! 