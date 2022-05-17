using Grpc.Core;
using gRPCProto;

namespace gRPCApi.Services;

public class HelloWorldService : HelloWorld.HelloWorldBase
{
    public override Task<HelloWorldResponse> SayHelloWorld(HelloWorldRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloWorldResponse
        {
            Name = "Hello " + request.Name,
        });
    }

    public override async Task SayHelloWorldSS(HelloWorldRequest request, IServerStreamWriter<HelloWorldResponse> responseStream, ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await responseStream.WriteAsync(new HelloWorldResponse
            {
                Name = "Hello " + request.Name,
            });
        }
    }
}
