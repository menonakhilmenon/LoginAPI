using LoginAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoginAPI.Helpers
{
    public interface IDataAccess
    {
        Task<List<ClientInfo>> GetUserInfo();
        Task<ClientInfo> GetUserInfo(string userName);
    }
}