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
        Console.WriteLine($"準備寫入：{i}");
        await writer.WriteAsync(i);
        Console.WriteLine($"已寫入：{i}");
    }

    writer.Complete(); // 告知消費端：不會再有新資料了
}

static async Task ConsumeAsync(ChannelReader<int> reader)
{
    await Task.Delay(800); // 故意讓消費端慢一點，方便觀察背壓

    await foreach (var item in reader.ReadAllAsync())
    {
        Console.WriteLine($"消費：{item}");
        await Task.Delay(500); // 模擬處理速度較慢
    }

    Console.WriteLine("所有項目都已處理完畢。");
}
