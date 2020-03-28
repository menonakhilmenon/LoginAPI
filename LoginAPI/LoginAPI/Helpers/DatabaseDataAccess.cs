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

        public async Task<bool> SetUserActivation(string email,bool state)
        {
            return await _helper.CallStoredProcedureExec("SetUserActivation", new DynamicParameters()
                .AddParameter("userEmail",email)
                .AddParameter("state",state)
                ) > 0;
        }

        public async Task<bool> CreateUser(ClientInfo clientInfo)
        {
            return await _helper.CallStoredProcedureExec("CreateUser", new DynamicParameters()
                .AddParameter("userID",clientInfo.userID)
                .AddParameter("userName",clientInfo.userName)
                .AddParameter("password",clientInfo.password)
                .AddParameter("email",clientInfo.email)
                .AddParameter("otp",clientInfo.otp)
                ) > 0;
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
        public async Task<ClientInfo> GetUserByID(string id)
        {
            return (await _helper.CallStoredProcedureQuery<ClientInfo>("GetUserByID", new DynamicParameters()
                    .AddParameter("userID", id))).FirstOrDefault();
        }
        public async Task<ClientInfo> GetUserByEmail(string email)
        {
            return (await _helper.CallStoredProcedureQuery<ClientInfo>("GetUserByEmail", new DynamicParameters()
                .AddParameter("userEmail",email))).FirstOrDefault();
            //using (IDbConnection conn = _helper.Connection)
            //{
            //    string sQuery = "SELECT * FROM user WHERE email = @email";
            //    conn.Open();
            //    var result = await conn.QueryAsync<ClientInfo>(sQuery, new { email = email });

            //    return result.FirstOrDefault();
            //}
        }
    }
}
