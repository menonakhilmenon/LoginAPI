using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoginAPI.GameServer;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Grpc.Core;

namespace LoginAPI.Grpc
{
    public class ServerCommunicator : IServerCommunicator
    {
        private readonly string serverAddress;
        public async Task<ServerReply> SendTokenToServer(string userID, string sessionKey)
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(serverAddress))
            {
                var gameServerClient = new GameServerAuth.GameServerAuthClient(channel);

                try 
                {
                    return await gameServerClient.SayHelloAsync(new UserReg()
                    {
                        SessionToken = sessionKey,
                        UserID = userID
                    });
                }
                catch 
                {
                    return new ServerReply() { ReplyMessage = -1 };
                }
            }
        }
        public ServerCommunicator(IConfiguration configuration)
        {
            serverAddress = configuration.GetValue<string>("GameServerIP");
        }
    }
}
