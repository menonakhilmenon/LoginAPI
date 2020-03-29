using LoginAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginAPI.Helpers
{
    public class DataAccess : IDataAccess
    {


        public Task<List<ClientInfo>> GetUserInfo()
        {
            return Task.FromResult(new List<ClientInfo>()
            {
                new ClientInfo()
                {
                    userName = "Akhil", password = "abc123", email = "abc@gmail.com", userID = Guid.NewGuid().ToString()
                },
                new ClientInfo()
                {
                    userName = "Adarsh", password = "abc123", email = "123@gmail.com", userID = Guid.NewGuid().ToString()
                }
            });
        }
        public Task<ClientInfo> GetUserByEmail(string userName)
        {
            List<ClientInfo> logininfo = new List<ClientInfo>()
            {
                new ClientInfo()
                {
                    userName = "Akhil", password = "abc123", email = "abc@gmail.com", userID = Guid.NewGuid().ToString()
                },
                new ClientInfo()
                {
                    userName = "Adarsh", password = "abc123", email = "123@gmail.com", userID = Guid.NewGuid().ToString()
                }
            };

            return Task.FromResult(logininfo[0]);
            //return logininfo.FirstOrDefault(x => x.userName == userName);
        }

        public Task<ClientInfo> GetUserByID(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ActivateUser(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateUser(ClientInfo clientInfo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeOTP(string userID, string otp)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetPassword(string userID, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangePasswordOTP(string userID, string password)
        {
            throw new NotImplementedException();
        }

        public bool ComparePassword(string unhashedPassword, string hashedPassword)
        {
            return unhashedPassword == hashedPassword;
        }
    }
}
