using System.Diagnostics;

// This sample creates HttpClient only once for the whole program lifetime,
// avoiding frequent creation and disposal in a short period of time.
using var httpClient = new HttpClient();

Console.WriteLine("Demonstrating HttpClient ResponseHeadersRead and streaming");

// Goal: download a larger file. This uses a public 10 MB test file that is less sensitive to network fluctuations.
string url = "https://proof.ovh.net/files/10Mb.dat";
string tempFilePath = Path.GetTempFileName();

Console.WriteLine($"Preparing to download file: {url}");
Console.WriteLine($"Target path: {tempFilePath}");

try
{
    var sw = Stopwatch.StartNew();
    using var cancellationSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    await DownloadLargeFileAsync(url, tempFilePath, cancellationSource.Token);
    sw.Stop();
    
    // Check the file size
    var fileInfo = new FileInfo(tempFilePath);
    Console.WriteLine($"\nDownload completed! Elapsed time: {sw.ElapsedMilliseconds} ms, File size: {fileInfo.Length / 1024 / 1024} MB");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Failed: the download timed out or was canceled. If the network is slow, adjust the CancellationTokenSource timeout.");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
finally
{
    // Clean up the temporary file
    if (File.Exists(tempFilePath))
    {
        File.Delete(tempFilePath);
        Console.WriteLine("The temporary file has been deleted.");
    }
}

async Task DownloadLargeFileAsync(
    string fileUrl,
    string destinationPath,
    CancellationToken cancellationToken = default)
{
    // Key argument: HttpCompletionOption.ResponseHeadersRead
    // This tells HttpClient to return as soon as the HTTP headers have been read.
    // Reading the body afterward is then controlled by the CancellationToken for cancellation or timeout.
    Console.WriteLine("Sending the HTTP request and waiting for the headers...");
    using var response = await httpClient.GetAsync(
        fileUrl,
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);
    
    // Ensure the HTTP status code is a successful 2xx
    response.EnsureSuccessStatusCode();

    // Get the total length of the incoming stream if the server provided Content-Length
    long? totalBytes = response.Content.Headers.ContentLength;
    string contentLengthText = totalBytes.HasValue
        ? $"{totalBytes.Value / 1024 / 1024} MB"
        : "Not provided";
    Console.WriteLine($"File size reported by the server (Content-Length): {contentLengthText}");
    
    Console.WriteLine("Starting to read from the network stream and continuously write to disk (backpressure-controlled streaming)...");
    
    // Get the live asynchronous data stream from the response
    using var networkStream = await response.Content.ReadAsStreamAsync(cancellationToken);
    
    // Create the local file stream
    // Key point: explicitly set useAsync: true so the runtime will prefer an asynchronous I/O path
    using var fileStream = new FileStream(
        destinationPath, 
        FileMode.Create, 
        FileAccess.Write, 
        FileShare.None, 
        bufferSize: 81920, 
        useAsync: true);

    // CopyToAsync automatically manages a fixed-size memory buffer underneath.
    // When the buffer fills with data from the network, it writes that data to disk.
    // While the disk write is in progress, network reads pause if the disk is slower,
    // which is backpressure and prevents memory usage from exploding.
    await networkStream.CopyToAsync(fileStream, 81920, cancellationToken);
}
