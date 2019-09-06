namespace Com.Kumulos.Abstractions
{
    public interface IKSConfig
    {
        IKSConfig AddKeys(string apiKey, string secretKey);
        IKSConfig EnableCrashReporting();
        IKSConfig SetSessionIdleTimeout(int timeoutSeconds);
        IKSConfig EnableInAppMessaging(InAppConsentStrategy consentStrategy);

        IKSConfig SetInAppDeepLinkHandler(IInAppDeepLinkHandler inAppDeepLinkHandler);
        
        string GetApiKey();
        string GetSecretKey();
    }
}
