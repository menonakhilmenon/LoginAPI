using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using LoginAPI.Models;

namespace LoginAPI.Helpers
{
    public class DatabaseDataAccess : IDataAccess
    {
        private IDatabaseHelper _helper;
        public DatabaseDataAccess(IDatabaseHelper helper)
        {
            _helper = helper;
        }
        public async Task<List<ClientInfo>> GetUserInfo()
        {
            using (IDbConnection conn = _helper.Connection)
            {
                string sQuery = "SELECT * FROM user";
                conn.Open();
                var result = await conn.QueryAsync<ClientInfo>(sQuery);
                return result.ToList();

            }
        }
        public async Task<ClientInfo> GetUserInfo(string userName)
        {
            using (IDbConnection conn = _helper.Connection)
            {
                string sQuery = "SELECT * FROM user WHERE username = @username";
                conn.Open();
                var result = await conn.QueryAsync<ClientInfo>(sQuery, new { username = userName });
                ClientInfo clientInfo = result.FirstOrDefault();
                if (result.Count() == 0)
                {
                    return null;
                }
                return new ClientInfo()
                {
                    userName = clientInfo.userName,
                    password = clientInfo.password,
                    email = clientInfo.email,
                    userID = clientInfo.userID
                };
            }
        }
    }
}
