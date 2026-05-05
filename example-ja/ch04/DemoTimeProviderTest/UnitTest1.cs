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
        // CancellationTokenSource コンストラクターへ TimeProvider を渡す。
        // これは new CancellationTokenSource(delay) に似ているが、
        // テスト可能になっている。
        var seconds = TimeSpan.FromSeconds(30);
        using var cts = new CancellationTokenSource(seconds, timeProvider);
        
        // 外部トークンもリンクする
        // (呼び出し元がキャンセルした場合、こちらもキャンセルすべきだから)。
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, cts.Token);

        try 
        {
            await DoActualWorkAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (cts.Token.IsCancellationRequested && !token.IsCancellationRequested)
        {
            // タイムアウト用トークンがキャンセルされ、
            // 外部トークンはキャンセルされていない場合だけ、タイムアウトとして扱う。
            throw new TimeoutException("操作がタイムアウトしました。");
        }
    }

    private async Task DoActualWorkAsync(CancellationToken token)
    {
        // 時間のかかる作業をシミュレートする。
        await Task.Delay(TimeSpan.FromSeconds(60), timeProvider, token);
    }
}

public class UnitTest1
{
    [Fact]
    public async Task Should_Throw_TimeoutException_When_Time_Passes()
    {
        // 準備
        var fakeTime = new FakeTimeProvider();
        var service = new MyService(fakeTime);

        // 実行
        var task = service.DoWorkWithTimeoutAsync(CancellationToken.None);

        // 検証: タスクはまだ完了していないはず。
        Assert.False(task.IsCompleted);

        // 実行: 時間を 30 秒 + 1 tick だけ早送りする。
        fakeTime.Advance(TimeSpan.FromSeconds(30) + TimeSpan.FromTicks(1));

        // 検証: タスクは TimeoutException で失敗するはず。
        await Assert.ThrowsAsync<TimeoutException>(() => task);
    }

    [Fact]
    public async Task Should_Keep_Cancellation_Semantics_When_External_Token_Is_Canceled()
    {
        // 準備
        var fakeTime = new FakeTimeProvider();
        var service = new MyService(fakeTime);
        using var cts = new CancellationTokenSource();

        // 実行
        var task = service.DoWorkWithTimeoutAsync(cts.Token);
        cts.Cancel();

        // 後でタイムアウトが起きるはずだったとしても、
        // TimeoutException と誤判定してはいけない。
        fakeTime.Advance(TimeSpan.FromSeconds(30) + TimeSpan.FromTicks(1));

        // 検証
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    }
}
