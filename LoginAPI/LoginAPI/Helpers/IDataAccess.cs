using LoginAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoginAPI.Helpers
{
    public interface IDataAccess
    {
        Task<List<ClientInfo>> GetUserInfo();
        Task<ClientInfo> GetUserByEmail(string email);
        Task<ClientInfo> GetUserByID(string id);
        Task<bool> SetUserActivation(string email,bool state);
        Task<bool> CreateUser(ClientInfo clientInfo);
    }
}