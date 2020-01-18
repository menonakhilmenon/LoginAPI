using System;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

namespace SessionKeyManager
{
    public class HashedSessionKeyManager : ISessionKeyManager
    {
        private Random random;
        private HashAlgorithm algorithm;

        private int counter;
        public HashedSessionKeyManager()
        {
            random = new Random();
            algorithm = SHA256.Create();
            counter = 0;
        }
        public string GenerateNewSessionKey(string userID)
        {
            var s = $"{userID}.{counter}.{random.Next()}";
            Interlocked.Increment(ref counter);
            return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(s)));
        }
        public string RefreshSessionKey(string userID) 
        {
            return GenerateNewSessionKey(userID);
        }
        public void ReleaseSessionKey(string key)
        {

        }
    }
}
