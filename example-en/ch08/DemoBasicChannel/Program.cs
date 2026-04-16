using System;
using System.Threading.Channels;
using System.Threading.Tasks;

var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(3)
{
    FullMode = BoundedChannelFullMode.Wait,
    SingleWriter = true,
    SingleReader = true
});

Task producer = ProduceAsync(channel.Writer);
Task consumer = ConsumeAsync(channel.Reader);

await Task.WhenAll(producer, consumer);

static async Task ProduceAsync(ChannelWriter<int> writer)
{
    for (int i = 1; i <= 5; i++)
    {
        Console.WriteLine($"Ready to write: {i}");
        await writer.WriteAsync(i);
        Console.WriteLine($"Written: {i}");
    }

    writer.Complete(); // Notify the consumer that no more data will arrive
}

static async Task ConsumeAsync(ChannelReader<int> reader)
{
    await Task.Delay(800); // Intentionally slow down the consumer so the backpressure is easier to observe

    await foreach (var item in reader.ReadAllAsync())
    {
        Console.WriteLine($"Consume: {item}");
        await Task.Delay(500); // Simulate slower processing
    }

    Console.WriteLine("All items have been processed.");
}
