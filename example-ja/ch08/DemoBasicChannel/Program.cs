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
        Console.WriteLine($"書き込み準備完了: {i}");
        await writer.WriteAsync(i);
        Console.WriteLine($"書き込み済み: {i}");
    }

    writer.Complete(); // これ以上データが来ないことをコンシューマーへ通知する
}

static async Task ConsumeAsync(ChannelReader<int> reader)
{
    await Task.Delay(800); // バックプレッシャーを観察しやすくするため、意図的にコンシューマーを遅くする

    await foreach (var item in reader.ReadAllAsync())
    {
        Console.WriteLine($"消費: {item}");
        await Task.Delay(500); // 遅めの処理をシミュレートする
    }

    Console.WriteLine("すべての項目を処理しました。");
}
