using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace Chat.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {            
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Messenger.MessengerClient(channel);

            var join = await client.JoinAsync(new JoinRegister 
            { 
                Name = Console.ReadLine() 
            });
            
            var call = client.SendMessage();
            var callTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {                    
                    if (response.IsJoin)
                    {
                        Console.WriteLine($"{response.Name} поключился к чату!");
                    }
                    else if (response.IsDisconnect)
                    {
                        Console.WriteLine($"{response.Name} покинул чат!");
                    }
                    else
                    {
                        Console.WriteLine($"{response.Name} : {response.Content}");
                    }
                }
            });

            await call.RequestStream.WriteAsync(new Message
            {
                Name = join.Name,                                             
                IsJoin = true
            });

            string message = Console.ReadLine();

            while(message != string.Empty)
            {
                await call.RequestStream.WriteAsync(
                    new Message
                    {
                        Name = join.Name,
                        Content = message
                    });

                message = Console.ReadLine();
            }

            await call.RequestStream.WriteAsync(new Message
            {
                Name = join.Name,
                IsDisconnect = true
            });

            await call.RequestStream.CompleteAsync();
            await client.DisconnectAsync(new DisconnectRegister
            {
                Name = join.Name
            });
        }
    }
}
