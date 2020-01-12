namespace SessionKeyManager
{
    public interface ISessionKeyManager
    {
        string GenerateNewSessionKey();
        void ReleaseSessionKey(string key);
    }
}