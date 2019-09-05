using System;
using Com.Kumulos.Abstractions;
using Foundation;

namespace Com.Kumulos
{
    public class KSConfigImplementation : IKSConfig
    {
        private string apiKey, secretKey;
        private bool enableCrashReporting;
        private InAppConsentStrategy consentStrategy = InAppConsentStrategy.NotEnabled;

        private iOS.KSInAppDeepLinkHandlerBlock deepLinkHandler;
        private iOS.KSPushOpenedHandlerBlock notificationHandler;
        private iOS.KSPushReceivedInForegroundHandlerBlock receivedHandler;

        public IKSConfig AddKeys(string apiKey, string secretKey)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;

            return this;
        }

        public IKSConfig EnableCrashReporting()
        {
            enableCrashReporting = true;
            return this;
        }

        public IKSConfig EnableInAppMessaging(InAppConsentStrategy consentStrategy)
        {
            this.consentStrategy = consentStrategy;
            return this;
        }

        public IKSConfig SetInAppDeepLinkHandler(iOS.KSInAppDeepLinkHandlerBlock deepLinkHandler)
        {
            this.deepLinkHandler = deepLinkHandler;
            return this;
        }

        public IKSConfig SetPushOpenedHandler(iOS.KSPushOpenedHandlerBlock notificationHandler)
        {
            this.notificationHandler = notificationHandler;
            return this;
        }

        public IKSConfig SetPushReceivedInForegroundHandler(iOS.KSPushReceivedInForegroundHandlerBlock receivedHandler)
        {
            this.receivedHandler = receivedHandler;
            return this;
        }

        public iOS.KSConfig Build()
        {
            var specificConfig = iOS.KSConfig.ConfigWithAPIKey(apiKey, secretKey);

            if (enableCrashReporting)
            {
                specificConfig.EnableCrashReporting();
            }

            if (consentStrategy != InAppConsentStrategy.NotEnabled)
            {
                specificConfig.EnableInAppMessaging(GetInAppConsentStrategy());
            }

            if (deepLinkHandler != null)
            {
                specificConfig.SetInAppDeepLinkHandler(deepLinkHandler);
            }

            if (notificationHandler != null)
            {
                specificConfig.SetPushOpenedHandler(notificationHandler);
            }

            if (receivedHandler != null)
            {
                specificConfig.SetPushReceivedInForegroundHandler(receivedHandler);
            }

            var sdkKeys = new object[] { "id", "version" };
            var sdkValues = new object[] { Consts.SDK_TYPE, Consts.SDK_VERSION };

            var sdkInfo = NSDictionary.FromObjectsAndKeys(sdkValues, sdkKeys);

            specificConfig.SetSdkInfo(sdkInfo);

            var runtimeKeys = new object[] { "id", "version" };
            var runtimeValues = new object[] { Consts.RUNTIME_TYPE, ObjCRuntime.Constants.Version };

            var runtimeInfo = NSDictionary.FromObjectsAndKeys(runtimeValues, runtimeKeys);

            specificConfig.SetRuntimeInfo(runtimeInfo);

            return specificConfig;
        }

        private iOS.KSInAppConsentStrategy GetInAppConsentStrategy()
        {
            if (consentStrategy == InAppConsentStrategy.AutoEnroll)
            {
                return iOS.KSInAppConsentStrategy.AutoEnroll;
            }

            if (consentStrategy == InAppConsentStrategy.ExplicitByUser)
            {
                return iOS.KSInAppConsentStrategy.ExplicitByUser;
            }

            throw new Exception("Invalid InAppConsent strategy");
        }

        public string GetApiKey()
        {
            return apiKey;
        }

        public string GetSecretKey()
        {
            return secretKey;
        }
    }
}
