using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LoginAPI.Models;

namespace LoginAPI.Helpers
{
    public class DatabaseDataAccess : IDataAccess
    {
        private IDatabaseHelper _helper;
        private SHA256 hasher;
        public DatabaseDataAccess(IDatabaseHelper helper)
        {
            _helper = helper;
            hasher = SHA256.Create();
        }

        public async Task<bool> ActivateUser(string email)
        {
            return await _helper.CallStoredProcedureExec("ActivateUser", new DynamicParameters()
                .AddParameter("userEmail", email)
                ) > 0;
        }

        public async Task<bool> CreateUser(ClientInfo clientInfo)
        {
            return await _helper.CallStoredProcedureExec("CreateUser", new DynamicParameters()
                .AddParameter("userID",clientInfo.userID)
                .AddParameter("userName",clientInfo.userName)
                .AddParameter("password", GetPasswordHash(clientInfo.password))
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
        }

        public async Task<bool> ChangeOTP(string userID, string otp)
        {
            return (await _helper.CallStoredProcedureExec("ChangeOTP", new DynamicParameters()
                .AddParameter("userID", userID)
                .AddParameter("otp",otp))) > 0;
        }

        public async Task<bool> SetPassword(string userID, string password)
        {
            return (await _helper.CallStoredProcedureExec("SetPassword", new DynamicParameters()
                    .AddParameter("userID", userID)
                    .AddParameter("newPassword", GetPasswordHash(password)))) > 0;
        }

        public async Task<bool> ChangePasswordOTP(string userID, string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                return (await _helper.CallStoredProcedureExec("ChangePasswordOTP", new DynamicParameters()
                    .AddParameter("userID", userID)
                    .AddParameter("passwordOTP", password))) > 0;
            }
            return false;
        }

        public bool ComparePassword(string unhashedPassword, string hashedPassword)
        {
            return hashedPassword == GetPasswordHash(unhashedPassword);
        }
        public string GetPasswordHash(string password) 
        {
            return Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
