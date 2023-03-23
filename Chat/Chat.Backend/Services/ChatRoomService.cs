using Chat.Backend.Models;
using Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Backend.Services
{
    public class ChatRoomService
    {
        private readonly List<User> _users = new();

        public Task<string> Join(User user)
        {
            _users.Add(user);
            return Task.FromResult(user.Name);
        }
        
        public void Remove(string name)
        {
            User user = _users.FirstOrDefault(u => u.Name == name);
            _users.Remove(user);
        }

        public async Task BroadcastMessageAsync(Message response)
        {
            foreach(var user in _users.Where(u => u.Name != response.Name))
            {
                await user.Response.WriteAsync(response);
            }
        }

        public void ConnectUser(string name, IAsyncStreamWriter<Message> response)
        {
            User user = _users.FirstOrDefault(u => u.Name == name);
            user.Response = response;
        }
    }
}
