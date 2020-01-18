namespace SessionKeyManager
{
    public interface ISessionKeyManager
    {
        string GenerateNewSessionKey(string userID);
        void ReleaseSessionKey(string key);
    }
}