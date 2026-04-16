// Demonstrates TaskStatus values and the convenient state properties

Console.WriteLine("Creating and starting a task that will intentionally fail...");

Task myTask = Task.Run(() =>
{
    Console.WriteLine("Task started and is about to throw an exception...");
    Thread.Sleep(500);
    throw new InvalidOperationException("Oops, the task failed!");
});                 

try
{
    // Intentionally wait for the task to complete so we can observe its final state
    myTask.Wait();
}
catch (AggregateException)
{
    // When Wait() is used on a failed Task,
    // the exception is wrapped in an AggregateException.
    // We catch and ignore it here so we can observe the task's final state.
}

Console.WriteLine($"Final task status: {myTask.Status}");
Console.WriteLine($"IsFaulted: {myTask.IsFaulted}");
Console.WriteLine($"IsCompleted: {myTask.IsCompleted}");
