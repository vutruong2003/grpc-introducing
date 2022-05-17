// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using gRPCCodeFirstBase.Contracts;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.Configuration;

while (true)
{
    Console.WriteLine("Enter secret key");
    var key = Console.ReadLine();

    if (key == "exit")
    {
        break;
    }

    try
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7160");

        var invoker = channel.Intercept(new ClientInterceptor(key));

        var client = ClientFactory.Default.CreateClient<IChatMessageService>(invoker);

        var reply = await client.SendMessage(new ChatMessage
        {
            Message = "Hello everyone",
            UserId = "Test"
        }, new CallContext(new CallOptions(new Metadata())));

        Console.WriteLine($"Greeting: {reply.UserId}");

        await channel.ShutdownAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}


public class ClientInterceptor : Interceptor
{
    private readonly string _key;
    public ClientInterceptor(string key)
    {
        _key = key;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        context.Options.Headers?.Add("SecretKey", _key);

        return base.AsyncUnaryCall(request, context, continuation);
    }
}