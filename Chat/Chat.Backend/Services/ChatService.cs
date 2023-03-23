using Chat.Backend.Models;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace Chat.Backend.Services
{
    public class ChatService : Messenger.MessengerBase
    {
        private readonly ChatRoomService _roomService;

        public ChatService(ChatRoomService roomService) =>
            _roomService = roomService;

        public override async Task<JoinRegister> Join(JoinRegister request, ServerCallContext context)
        {
            Console.WriteLine($"=== {request.Name} JOIN ===");
            
            return new JoinRegister
            {
                Name = await _roomService.Join(new User { Name = request.Name })
            };
        }

        public override async Task SendMessage(IAsyncStreamReader<Message> requestStream, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            await foreach(Message request in requestStream.ReadAllAsync())
            {
                _roomService.ConnectUser(request.Name, responseStream);
                await _roomService.BroadcastMessageAsync(
                    new Message
                    {
                        Name = request.Name,                        
                        Content = request.Content,
                        IsJoin = request.IsJoin,
                        IsDisconnect = request.IsDisconnect
                    });

                Console.WriteLine($"=== {request.Name} SEND {request.Content} ===");
            }
        }

        public override Task<DisconnectRegister> Disconnect(DisconnectRegister request, ServerCallContext context)
        {
            _roomService.Remove(request.Name);
            Console.WriteLine($"=== {request.Name} DISCONNECT ===");

            return Task.FromResult(new DisconnectRegister
            {
                Name = request.Name
            });
        }
    }
}
