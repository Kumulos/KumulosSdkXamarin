namespace Com.Kumulos.Abstractions
{
    public interface IKSConfig
    {
        IKSConfig AddKeys(string apiKey, string secretKey);
        IKSConfig EnableCrashReporting();
        IKSConfig EnableInAppMessaging(InAppConsentStrategy consentStrategy);
        
        string GetApiKey();
        string GetSecretKey();
    }
}
