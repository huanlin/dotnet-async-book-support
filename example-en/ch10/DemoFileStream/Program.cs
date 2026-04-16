using System.Diagnostics;

Console.WriteLine("Demonstrating asynchronous FileStream I/O and the concept of backpressure");

string sourcePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
string destinationPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

try
{
    // 1. First create a dummy test file of about 500 MB
    Console.WriteLine($"Creating a source file for the test ({sourcePath})...");
    CreateDummyFile(sourcePath, 500 * 1024 * 1024); // 500 MB
    
    Console.WriteLine($"Starting the asynchronous file copy to ({destinationPath})...");
    var sw = Stopwatch.StartNew();
    
    await CopyFileWithAsyncStream(sourcePath, destinationPath);
    
    sw.Stop();
    Console.WriteLine($"\nCopy completed! Elapsed time: {sw.ElapsedMilliseconds} ms");
}
finally
{
    // Clean up the temporary files
    if (File.Exists(sourcePath)) File.Delete(sourcePath);
    if (File.Exists(destinationPath)) File.Delete(destinationPath);
}

// Generate the test file with synchronous writes
void CreateDummyFile(string path, int sizeInBytes)
{
    var buffer = new byte[81920];
    new Random().NextBytes(buffer);
    
    using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
    for (int i = 0; i < sizeInBytes / buffer.Length; i++)
    {
        fs.Write(buffer, 0, buffer.Length);
    }
}

async Task CopyFileWithAsyncStream(string source, string dest)
{
    // Key point 1: explicitly specify useAsync: true for reading
    using var sourceStream = new FileStream(
        source, 
        FileMode.Open, 
        FileAccess.Read, 
        FileShare.Read, 
        bufferSize: 81920, 
        useAsync: true);

    // Key point 2: explicitly specify useAsync: true for writing
    using var destinationStream = new FileStream(
        dest, 
        FileMode.Create, 
        FileAccess.Write, 
        FileShare.None, 
        bufferSize: 81920, 
        useAsync: true);

    // Key point 3: backpressure control provided by CopyToAsync
    // It does not read the full 500 MB into RAM at once. Instead it uses a fixed-size buffer, such as 81920 bytes.
    // Fill the buffer -> pause reading -> write to disk -> wait for the write -> continue reading.
    // That keeps memory usage around the fixed buffer size instead of growing linearly with the file.
    
    // We can also pass our own buffer size to CopyToAsync
    await sourceStream.CopyToAsync(destinationStream, bufferSize: 81920);
}
