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
        Task<bool> ActivateUser(string email);
        Task<bool> CreateUser(ClientInfo clientInfo);
        Task<bool> ChangeOTP(string userID, string otp);
        Task<bool> SetPassword(string userID, string password);
        Task<bool> ChangePasswordOTP(string userID, string password);
        bool ComparePassword(string unhashedPassword, string hashedPassword);
    }
}