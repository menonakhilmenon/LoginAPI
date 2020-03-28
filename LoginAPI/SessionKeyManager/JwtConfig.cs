using System;
using System.Collections.Generic;
using System.Text;

namespace SessionKeyManager
{
    public class JwtConfig
    {
        /// <summary>
        /// The issuer of the key
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// The key used for signing the Jwt
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// The duration the Jwt will be valid for in seconds
        /// </summary>
        public int Duration { get; set; } = 600;
    }
}
