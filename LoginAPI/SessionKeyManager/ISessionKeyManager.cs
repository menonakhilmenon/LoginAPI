namespace SessionKeyManager
{
    public interface ISessionKeyManager
    {
        string GenerateNewSessionKey(string userID);
        string RefreshSessionKey(string userID);
        void ReleaseSessionKey(string userID);
    }
}