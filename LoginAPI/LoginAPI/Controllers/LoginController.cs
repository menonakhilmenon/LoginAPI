using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoginAPI.Models;
using Microsoft.AspNetCore.Mvc;
using LoginAPI.Helpers;
using SessionKeyManager;
using Microsoft.AspNetCore.Authorization;

namespace LoginAPI.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;
        private readonly ISessionKeyManager _sessionKeyManager;

        public LoginController(IDataAccess dataAccess, ISessionKeyManager sessionKeyManager)
        {
            _dataAccess = dataAccess;
            _sessionKeyManager = sessionKeyManager;
        }

        [Authorize(AuthenticationSchemes = JwtExtensionsAndConstants.JwtAuthenticationScheme)]
        [HttpPost]
        [Route("refresh")]
        public ClientReply Refresh()
        {
            var id = HttpContext.GetUserIDFromJWTHeader();
            Console.WriteLine($"{id} has requested refresh token");
            var sessionKey = _sessionKeyManager.RefreshSessionKey(id);
            if (string.IsNullOrEmpty(sessionKey)) 
            {
                return new ClientReply
                {
                    token = "",
                    Error = (int)ErrorMessage.Unknown
                };
            }
            return new ClientReply { token = sessionKey, Error = (int)ErrorMessage.NoError };
        }



        [Route("login")]
        [HttpPost]
        public async Task<ClientReply> Auth([FromBody]ClientLoginInfo loginInfo)
        {
            Console.WriteLine($"{loginInfo.userName} has requested login..");
            var op = await _dataAccess.GetUserByEmail(loginInfo.userName);
            if (op.password == loginInfo.password)
            {
                string sessKey = _sessionKeyManager.GenerateNewSessionKey(op.userID);

                return new ClientReply()
                {
                    token = sessKey,
                    Error = (int)ErrorMessage.NoError
                };
            }
            else 
            {
                return new ClientReply()
                {
                    token = "Invalid Credentials",
                    Error = (int)ErrorMessage.InvalidCredentials
                };
            }
        }

    }
}