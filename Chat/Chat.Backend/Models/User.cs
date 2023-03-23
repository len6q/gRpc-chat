using Grpc.Core;

namespace Chat.Backend.Models
{
    public class User
    {
        public string Name { get; set; }
        public IAsyncStreamWriter<Message> Response { get; set; }
        public bool IsJoin { get; set; }
        public bool IsDisconnect { get; set; }
    }
}
