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
        // Pass TimeProvider to the CancellationTokenSource constructor.
        // This is similar to new CancellationTokenSource(delay),
        // but now it is testable.
        var seconds = TimeSpan.FromSeconds(30);
        using var cts = new CancellationTokenSource(seconds, timeProvider);
        
        // Link the external token too
        // (if the caller cancels, we should cancel as well).
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, cts.Token);

        try 
        {
            await DoActualWorkAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (cts.Token.IsCancellationRequested && !token.IsCancellationRequested)
        {
            // Treat it as a timeout only when the timeout token was canceled
            // and the external token was not.
            throw new TimeoutException("The operation timed out.");
        }
    }

    private async Task DoActualWorkAsync(CancellationToken token)
    {
        // Simulate work that takes a long time.
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

        // Assert: the task should not be completed yet.
        Assert.False(task.IsCompleted);

        // Act: fast-forward time by 30 seconds plus 1 tick.
        fakeTime.Advance(TimeSpan.FromSeconds(30) + TimeSpan.FromTicks(1));

        // Assert: the task should now fail with TimeoutException.
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

        // Even if the timeout would happen later, it should not be mistaken
        // for TimeoutException.
        fakeTime.Advance(TimeSpan.FromSeconds(30) + TimeSpan.FromTicks(1));

        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    }
}
