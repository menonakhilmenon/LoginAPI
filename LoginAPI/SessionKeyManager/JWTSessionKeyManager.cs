using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace SessionKeyManager
{
    public class JWTSessionKeyManager : ISessionKeyManager
    {
        private readonly HashAlgorithm hashAlgorithm;
        private readonly IConfiguration _config;
        private readonly SigningCredentials credentials;
        private readonly string issuer;

        public const string USERID_STRING = "userID";

        private readonly HashSet<string> refreshTokens = new HashSet<string>();


        public JWTSessionKeyManager(IConfiguration config)
        {
            _config = config;

            issuer = _config["Jwt:Issuer"];
            hashAlgorithm = SHA256.Create();

            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var securityKey = new SymmetricSecurityKey(hashAlgorithm.ComputeHash(key));

            credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }
        
        public string GenerateNewSessionKey(string userID)
        {
            refreshTokens.Add(userID);

            string key = GenerateJSONWebToken(userID);
            Console.WriteLine(key);
            return key;
        }

        public void ReleaseSessionKey(string userID)
        {
            if (refreshTokens.Contains(userID)) 
            {
                refreshTokens.Remove(userID);
            }
        }

        public string RefreshSessionKey(string userID) 
        {
            if (refreshTokens.Contains(userID)) 
            {
                return GenerateJSONWebToken(userID);
            }
            return "Session key was not found";
        }


        private string GenerateJSONWebToken(string userInfo)
        {
            
            var token = new JwtSecurityToken(
              issuer : this.issuer,
              claims: new[] {new Claim(USERID_STRING,userInfo)},
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
