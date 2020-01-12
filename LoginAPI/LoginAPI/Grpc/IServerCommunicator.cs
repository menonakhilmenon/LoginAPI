using LoginAPI.GameServer;
using System.Threading.Tasks;

namespace LoginAPI.Grpc
{
    public interface IServerCommunicator
    {
        Task<ServerReply> SendTokenToServer(string userID, string sessionKey);
    }
}