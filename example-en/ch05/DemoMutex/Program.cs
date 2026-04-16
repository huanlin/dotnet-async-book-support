using System;
using System.Threading;

Console.WriteLine(
    "Demo: use a named Mutex to detect whether another instance is already running.");

// Create a named mutex. The name should be a unique cross-platform-friendly
// string to avoid collisions.
// Limit it explicitly to the current user and current session so processes
// from other users or sessions cannot interfere with the same mutex name.
using Mutex mutex = new Mutex(
    false,
    "com.example.myawesomeapp.single-instance.A1B2C3D4",
    new NamedWaitHandleOptions
    {
        CurrentUserOnly = true,
        CurrentSessionOnly = true
    });

// Try to acquire the lock. Wait 0 milliseconds
// so the call returns immediately.
try
{
    if (!mutex.WaitOne(0))
    {
        Console.WriteLine(
            "The application is already running. Do not open it twice.");
        return; // Exit the application
    }

    try
    {
        Console.WriteLine(
            "Application started successfully. Press Enter to exit...");
        Console.ReadLine();
    }
    finally
    {
        // Make sure the mutex is released when leaving.
        mutex.ReleaseMutex();
    }
}
catch (AbandonedMutexException)
{
    // Defensive handling: if the previous owner of the mutex ended abnormally,
    // and the named mutex object stayed alive because waiters or open handles
    // still existed, then the next successful waiter may receive
    // AbandonedMutexException.
    // Receiving this exception means the current instance now owns the mutex,
    // but it does not guarantee that the protected shared state is still safe
    // or consistent.
    Console.WriteLine(
        "Detected that the previous application instance did not close cleanly (abandoned mutex).");

    // ... state validation or repair logic could go here ...
    try
    {
        Console.WriteLine(
            "Application started successfully. Press Enter to exit...");
        Console.ReadLine();
    }
    finally
    {
        mutex.ReleaseMutex();
    }
}
