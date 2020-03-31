using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginAPI.Models
{
    public class ClientInfo
    {
        public string userID;
        public string userName;
        public string email;
        public string password;
        public bool activated;
        public string otp;
        public string forgetPasswordOtp;
    }
}
