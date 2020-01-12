﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoginAPI.Models;
using Microsoft.AspNetCore.Mvc;
using LoginAPI.Helpers;
using LoginAPI.Grpc;
using SessionKeyManager;

namespace LoginAPI.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;
        private readonly IServerCommunicator _serverCommunicator;
        private readonly ISessionKeyManager _sessionKeyManager;

        public LoginController(IDataAccess dataAccess, IServerCommunicator serverCommunicator, ISessionKeyManager sessionKeyManager)
        {
            _dataAccess = dataAccess;
            _serverCommunicator = serverCommunicator;
            _sessionKeyManager = sessionKeyManager;
        }

        [Route("login")]
        [HttpPost]
        public async Task<ClientReply> Auth([FromBody]ClientLoginInfo loginInfo)
        {
            Console.WriteLine($"{loginInfo.userName} has requested login..");
            var op = await _dataAccess.GetUserInfo(loginInfo.userName);
            if (op.password == loginInfo.password)
            {
                string sessKey = _sessionKeyManager.GenerateNewSessionKey();
                var res = await _serverCommunicator.SendTokenToServer(loginInfo.userName, sessKey);

                if (res.ReplyMessage == 0)
                {
                    return new ClientReply()
                    {
                        token = sessKey,
                        Error = (int)ErrorMessage.NoError
                    };
                }
                else
                {
                    return new ClientReply
                    {
                        token = "MasterServer Timed out",
                        Error = (int)ErrorMessage.Timeout
                    };
                }
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