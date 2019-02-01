namespace Com.Kumulos.Abstractions
{
    public interface IKSConfig
    {
        IKSConfig AddKeys(string apiKey, string secretKey);
        IKSConfig EnableCrashReporting();
        IKSConfig SetSessionIdleTimeout(int timeoutSeconds);

        string GetApiKey();
        string GetSecretKey();
    }
}
