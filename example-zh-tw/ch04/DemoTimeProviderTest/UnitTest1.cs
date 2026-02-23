using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Time.Testing;

namespace DemoTimeProviderTest;

public class MyService
{
    private readonly TimeProvider _timeProvider;

    public MyService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task DoWorkWithTimeoutAsync(CancellationToken token)
    {
        // 使用 CancellationTokenSource 的建構函式傳入 TimeProvider
        // 這與 new CancellationTokenSource(delay) 類似，但它是可測試的！
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30), _timeProvider);
        
        // 連結外部傳入的 token (如果外部取消，我們也要取消)
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, cts.Token);

        try 
        {
            await DoActualWorkAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
        {
             // 區分是「逾時取消」還是「使用者取消」
             throw new TimeoutException("操作已逾時。");
        }
    }

    private async Task DoActualWorkAsync(CancellationToken token)
    {
        // 模擬需要延遲很長時間的工作
        await Task.Delay(TimeSpan.FromSeconds(60), _timeProvider, token);
    }
}

public class UnitTest1
{
    [Fact]
    public async Task Should_Throw_TimeoutException_When_Time_Passes()
    {
        // Arrange
        var fakeTime = new FakeTimeProvider();
        var service = new MyService(fakeTime);

        // Act
        var task = service.DoWorkWithTimeoutAsync(CancellationToken.None);

        // Assert: 此時任務應該還沒完成
        Assert.False(task.IsCompleted);

        // Act: 讓時間快轉 30 秒 + 1 tick
        fakeTime.Advance(TimeSpan.FromSeconds(30.1));

        // Assert: 任務應該因為逾時而拋出 TimeoutException
        await Assert.ThrowsAsync<TimeoutException>(async () => await task);
    }
}
