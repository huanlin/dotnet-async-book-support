using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

// 優先度付きチャネルを作成する
var prioritizedChannel = Channel.CreateUnboundedPrioritized<WorkItem>(
    new UnboundedPrioritizedChannelOptions<WorkItem>
    {
        Comparer = new PriorityComparer()
    });

// 2 つの作業項目を書き込む
await prioritizedChannel.Writer.WriteAsync(new WorkItem("通常の作業", 1));
await prioritizedChannel.Writer.WriteAsync(new WorkItem("緊急の作業", 99));

// 読み取る。"通常の作業" が先に書き込まれていても、"緊急の作業" が先に読み取られる
// 優先度が高いため。
var item = await prioritizedChannel.Reader.ReadAsync();
Console.WriteLine($"作業を処理中: {item.Name}"); // 出力: 緊急の作業

var item2 = await prioritizedChannel.Reader.ReadAsync();
Console.WriteLine($"作業を処理中: {item2.Name}"); // 出力: 通常の作業

// 優先度を含む作業項目を定義する
public record WorkItem(string Name, int Priority);

// 比較器を実装する: Priority が高いものを先にする (降順)
public class PriorityComparer : IComparer<WorkItem>
{
    public int Compare(WorkItem? x, WorkItem? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return 1;
        if (y is null) return -1;
        return y.Priority.CompareTo(x.Priority); // Priority の値が高いものから読み取られる
    }
}
