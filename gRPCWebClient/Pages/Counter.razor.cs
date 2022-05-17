using Grpc.Core;
using gRPCProto;
using Microsoft.AspNetCore.Components;

namespace gRPCWebClient.Pages;

public partial class Counter
{
    private int currentCount = 0;
    private CancellationTokenSource? cts;

    [Inject]
    protected gRPCProto.Counter.CounterClient CounterClient { get; set; }

    private async Task IncrementCount()
    {
        cts = new CancellationTokenSource();

        using var counterCall = CounterClient.StartCounter(new CounterRequest
        {
            Start = currentCount
        }, cancellationToken: cts.Token);

        try
        {
            await foreach (var message in counterCall.ResponseStream.ReadAllAsync())
            {
                currentCount = message.Count;
                StateHasChanged();
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            // Ignore exception from cancellation
        }
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
