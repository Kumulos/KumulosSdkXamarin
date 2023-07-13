namespace KumulosSDK.DotNet.Abstractions
{
    public interface IKSConfig
    {
        IKSConfig AddKeys(string apiKey, string secretKey);
        IKSConfig EnableCrashReporting();
        IKSConfig SetSessionIdleTimeout(int timeoutSeconds);
        IKSConfig EnableInAppMessaging(InAppConsentStrategy consentStrategy);
        IKSConfig EnableDeepLinking(string cname, IDeepLinkHandler deepLinkHandler);
        IKSConfig EnableDeepLinking(IDeepLinkHandler deepLinkHandler);

        IKSConfig SetInAppDeepLinkHandler(IInAppDeepLinkHandler inAppDeepLinkHandler);

        string GetApiKey();
        string GetSecretKey();
    }
}
