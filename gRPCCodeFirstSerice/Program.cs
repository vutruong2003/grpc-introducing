using Grpc.AspNetCore.Server;
using Grpc.Core;
using gRPCCodeFirstSerice.Middlewares;
using gRPCCodeFirstSerice.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddCodeFirstGrpc(
    options =>
    {
        options.Interceptors.Add<ExceptionInterceptor>();
        options.EnableDetailedErrors = true;
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ImplicitAuthorize", policy =>
    {
        policy.AddRequirements(new GrpcRequirement());
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, GrpcHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChatMessageService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    await next();
    // If response is unauthorized and the endpoint is a gRPC method then
    // return grpc-status permission denied instead

    if ((context.Response.StatusCode == StatusCodes.Status401Unauthorized || context.Response.StatusCode == StatusCodes.Status404NotFound) &&
        context.GetEndpoint()?.Metadata.GetMetadata<GrpcMethodMetadata>() != null)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.AppendTrailer("grpc-status", StatusCode.PermissionDenied.ToString("D"));
    }
});

app.Run();
