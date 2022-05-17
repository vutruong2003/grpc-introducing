using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using gRPCProto;

namespace gRPCClientShared;

public static class Startup
{
    public static GrpcChannel GetGrpcChannel(string gRPCHost)
    {
        var httpHandler = new GrpcWebHandler(new HttpClientHandler());

        return GrpcChannel.ForAddress(gRPCHost, new GrpcChannelOptions { HttpHandler = httpHandler });
    }

    public static void AddGrpcChannel(this IServiceCollection services)
    {
        services.AddSingleton(services =>
        {
            var config = services.GetRequiredService<IConfiguration>();
            var backendUrl = config["BackendUrl"];

            if (string.IsNullOrEmpty(backendUrl))
            {
                var navigationManager = services.GetRequiredService<NavigationManager>();
                backendUrl = navigationManager.BaseUri;
            }

            return GetGrpcChannel(backendUrl);
        });
    }

    public static Todo.TodoClient GetTodoClient(GrpcChannel channel)
    {
        return new Todo.TodoClient(channel);
    }

    public static void AddGrpc_TodoClient(this IServiceCollection services)
    {
        services.AddSingleton(services =>
        {
            var channel = services.GetRequiredService<GrpcChannel>();

            return GetTodoClient(channel);
        });
    }

    public static Counter.CounterClient GetCounterClient(GrpcChannel channel)
    {
        return new Counter.CounterClient(channel);
    }

    public static void AddGrpc_CounterClient(this IServiceCollection services)
    {
        services.AddSingleton(services =>
        {
            var channel = services.GetRequiredService<GrpcChannel>();

            return GetCounterClient(channel);
        });
    }

    public static Greeter.GreeterClient GetGreeterClient(GrpcChannel channel)
    {
        return new Greeter.GreeterClient(channel);
    }

    public static void AddGrpc_GreeterClient(this IServiceCollection services)
    {
        services.AddSingleton(services =>
        {
            var channel = services.GetRequiredService<GrpcChannel>();

            var client = new Greeter.GreeterClient(channel);

            return client;
        });
    }
}