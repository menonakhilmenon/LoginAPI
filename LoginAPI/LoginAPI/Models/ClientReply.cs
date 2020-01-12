using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginAPI.Models
{
    public class ClientReply
    {
        public string token { get; set; }
        public int Error { get; set; } = (int)ErrorMessage.Unknown;
    }
    public enum ErrorMessage
    {
        NoError,
        NetworkError,
        Timeout,
        InvalidCredentials,
        Unknown
    }
}
