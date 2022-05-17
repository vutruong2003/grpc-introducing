using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using gRPCWasmClient;
using gRPCWasmClient.Services;
using gRPCClientShared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddGrpcChannel();
builder.Services.AddGrpc_CounterClient();
builder.Services.AddGrpc_TodoClient();

builder.Services.AddSingleton<ITodoService, TodoService>();

await builder.Build().RunAsync();
