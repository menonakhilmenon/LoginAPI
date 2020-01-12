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
                    userName = "Akhil", password = "abc123", email = "abc@gmail.com", userID = 1
                },
                new ClientInfo()
                {
                    userName = "Adarsh", password = "abc123", email = "123@gmail.com", userID = 2
                }
            });
        }
        public Task<ClientInfo> GetUserInfo(string userName)
        {
            List<ClientInfo> logininfo = new List<ClientInfo>()
            {
                new ClientInfo()
                {
                    userName = "Akhil", password = "abc123", email = "abc@gmail.com", userID = 1
                },
                new ClientInfo()
                {
                    userName = "Adarsh", password = "abc123", email = "123@gmail.com", userID = 2
                }
            };

            return Task.FromResult(logininfo[0]);
            //return logininfo.FirstOrDefault(x => x.userName == userName);
        }
    }
}
