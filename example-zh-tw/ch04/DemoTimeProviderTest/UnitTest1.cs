using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Time.Testing;

namespace DemoTimeProviderTest;

public class MyService(TimeProvider timeProvider)
{
    public async Task DoWorkWithTimeoutAsync(CancellationToken token)
    {
        // 使用 CancellationTokenSource 的建構函式傳入 TimeProvider
        // 這與 new CancellationTokenSource(delay) 類似，但它是可測試的！
        var seconds = TimeSpan.FromSeconds(30);
        using var cts = new CancellationTokenSource(seconds, timeProvider);
        
        // 連結外部傳入的 token (如果外部取消，我們也要取消)
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, cts.Token);

        try 
        {
            await DoActualWorkAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (cts.Token.IsCancellationRequested && !token.IsCancellationRequested)
        {
            // 只有在 timeout token 已取消、且外部 token 尚未取消時，才視為逾時
            throw new TimeoutException("操作已逾時。");
        }
    }

    private async Task DoActualWorkAsync(CancellationToken token)
    {
        // 模擬需要延遲很長時間的工作
        await Task.Delay(TimeSpan.FromSeconds(60), timeProvider, token);
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
        fakeTime.Advance(TimeSpan.FromSeconds(30) + TimeSpan.FromTicks(1));

        // Assert: 任務應該因為逾時而拋出 TimeoutException
        await Assert.ThrowsAsync<TimeoutException>(() => task);
    }

    [Fact]
    public async Task Should_Keep_Cancellation_Semantics_When_External_Token_Is_Canceled()
    {
        // Arrange
        var fakeTime = new FakeTimeProvider();
        var service = new MyService(fakeTime);
        using var cts = new CancellationTokenSource();

        // Act
        var task = service.DoWorkWithTimeoutAsync(cts.Token);
        cts.Cancel();

        // 即使之後逾時也不應誤判成 TimeoutException
        fakeTime.Advance(TimeSpan.FromSeconds(30) + TimeSpan.FromTicks(1));

        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    }
}
