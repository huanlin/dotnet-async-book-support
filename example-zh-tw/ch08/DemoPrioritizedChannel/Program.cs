using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

// 建立優先級通道
var prioritizedChannel = Channel.CreateUnboundedPrioritized<WorkItem>(
    new UnboundedPrioritizedChannelOptions<WorkItem>
    {
        Comparer = new PriorityComparer()
    });

// 寫入兩個工作
await prioritizedChannel.Writer.WriteAsync(new WorkItem("一般工作", 1));
await prioritizedChannel.Writer.WriteAsync(new WorkItem("緊急工作", 99));

// 讀取：即使 "一般工作" 先寫入，但因為 "緊急工作" 優先級高，所以會先被讀出來！
var item = await prioritizedChannel.Reader.ReadAsync();
Console.WriteLine($"處理工作：{item.Name}"); // 輸出：緊急工作

var item2 = await prioritizedChannel.Reader.ReadAsync();
Console.WriteLine($"處理工作：{item2.Name}"); // 輸出：一般工作

// 定義工作項目，包含優先級
public record WorkItem(string Name, int Priority);

// 實作比較器：Priority 越高的越前面 (降冪排序)
public class PriorityComparer : IComparer<WorkItem>
{
    public int Compare(WorkItem x, WorkItem y)
    {
        // ret > 0: x > y (x 排前面)
        // ret < 0: x < y (y 排前面)
        return y.Priority.CompareTo(x.Priority);
    }
}
