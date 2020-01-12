using System;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

namespace SessionKeyManager
{
    public class HashedSessionKeyManager : ISessionKeyManager
    {
        private Random random;
        private SHA256 sha;

        private int counter;
        public HashedSessionKeyManager()
        {
            random = new Random();
            sha = SHA256.Create();
            counter = 0;
        }
        public string GenerateNewSessionKey()
        {
            var s = $"{counter}..{random.Next()}";
            Interlocked.Increment(ref counter);
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(s)));
        }

        public void ReleaseSessionKey(string key)
        {

        }
    }
}
