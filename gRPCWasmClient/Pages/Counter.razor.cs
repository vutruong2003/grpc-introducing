using Grpc.Core;
using Microsoft.AspNetCore.Components;

using gRPCProto;

namespace gRPCWasmClient.Pages;

public partial class Counter
{
    private int currentCount = 0;
    private CancellationTokenSource? cts;

    [Inject]
    protected gRPCProto.Counter.CounterClient CounterClient { get; set; }

    private async Task IncrementCount()
    {
        cts = new CancellationTokenSource();

        using var _counterCall = CounterClient.StartCounter(new CounterRequest
        {
            Start = currentCount
        }, cancellationToken: cts.Token);

        try
        {
            while (await _counterCall.ResponseStream.MoveNext())
            {
                var item = _counterCall.ResponseStream.Current;
                currentCount = item.Count;
                StateHasChanged();
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            // Ignore exception from cancellation
        }
    }

    private async Task ResetCount()
    {
        await CounterClient.ResetCounterAsync(new CounterRequest { Start = 0 });
    }

    private async Task ToggleCount()
    {
        await CounterClient.DoCountAsync(new CountRequest
        {
            Amount = 1
        });
    }

    private void StopCount()
    {
        cts?.Cancel();
        cts = null;
    }

    public void Dispose()
    {
        StopCount();
    }
}
