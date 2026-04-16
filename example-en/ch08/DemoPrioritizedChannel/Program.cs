using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

// Create a prioritized channel
var prioritizedChannel = Channel.CreateUnboundedPrioritized<WorkItem>(
    new UnboundedPrioritizedChannelOptions<WorkItem>
    {
        Comparer = new PriorityComparer()
    });

// Write two work items
await prioritizedChannel.Writer.WriteAsync(new WorkItem("Normal work", 1));
await prioritizedChannel.Writer.WriteAsync(new WorkItem("Urgent work", 99));

// Read them: even though "Normal work" was written first, "Urgent work" will be read first
// because it has a higher priority.
var item = await prioritizedChannel.Reader.ReadAsync();
Console.WriteLine($"Processing work: {item.Name}"); // Output: Urgent work

var item2 = await prioritizedChannel.Reader.ReadAsync();
Console.WriteLine($"Processing work: {item2.Name}"); // Output: Normal work

// Define a work item that includes a priority
public record WorkItem(string Name, int Priority);

// Implement a comparer: higher Priority comes first (descending order)
public class PriorityComparer : IComparer<WorkItem>
{
    public int Compare(WorkItem? x, WorkItem? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return 1;
        if (y is null) return -1;
        return y.Priority.CompareTo(x.Priority); // Higher Priority values are read first
    }
}
