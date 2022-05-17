using Grpc.Core;
using gRPCCodeFirstBase.Contracts;
using Microsoft.AspNetCore.Authorization;
using ProtoBuf.Grpc;

namespace gRPCCodeFirstSerice.Services;

[Authorize("ImplicitAuthorize")]
public class ChatMessageService : IChatMessageService
{
    public Task<ChatMessageReply> SendMessage(ChatMessage request, CallContext context = default)
    {
        return Task.FromResult(
           new ChatMessageReply
           {
               Message = request.Message,
               UserId = request.UserId + "(Sent)"
           });
    }
}