using ProtoBuf.Grpc;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace gRPCCodeFirstBase.Contracts
{
    [DataContract]
    public class ChatMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }

        [DataMember(Order = 2)]
        public string UserId { get; set; }
    }

    [DataContract]
    public class ChatMessageReply
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }

        [DataMember(Order = 2)]
        public string UserId { get; set; }
    }

    [ServiceContract]
    public interface IChatMessageService
    {
        [OperationContract]
        Task<ChatMessageReply> SendMessage(ChatMessage request, CallContext context = default);
    }
}
