namespace Com.Kumulos.Abstractions
{
    public interface IKSConfig
    {
        IKSConfig AddKeys(string apiKey, string secretKey);
        IKSConfig EnableCrashReporting();
        IKSConfig EnableInAppMessaging(InAppConsentStrategy consentStrategy);

        IKSConfig SetInAppDeepLinkHandler(IINAppDeepLinkHandler inAppDeepLinkHandler);
        
        string GetApiKey();
        string GetSecretKey();
    }
}
