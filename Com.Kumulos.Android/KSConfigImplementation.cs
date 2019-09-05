using System;
using Android.OS;
using Org.Json;

namespace Com.Kumulos
{
    public class KSConfigImplementation : Abstractions.IKSConfig
    {
        private string apiKey, secretKey;
        private Abstractions.InAppConsentStrategy consentStrategy = Abstractions.InAppConsentStrategy.NotEnabled;

        public Abstractions.IInAppDeepLinkHandler InAppDeepLinkHandler { get; private set; }

        public Abstractions.IKSConfig AddKeys(string apiKey, string secretKey)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;

            return this;
        }

        public Abstractions.IKSConfig EnableCrashReporting()
        {
            throw new NotImplementedException("Native crash reporting is not available on Android - please refer to the integration guide.");
        }

        public Abstractions.IKSConfig EnableInAppMessaging(Abstractions.InAppConsentStrategy consentStrategy)
        {
            this.consentStrategy = consentStrategy;
            return this;
        }

        public Abstractions.IKSConfig SetInAppDeepLinkHandler(Abstractions.IInAppDeepLinkHandler inAppDeepLinkHandler)
        {
            InAppDeepLinkHandler = inAppDeepLinkHandler;
            return this;
        }

        public Android.KumulosConfig GetConfig()
        {
            var specificConfig = new Android.KumulosConfig.Builder(apiKey, secretKey);

            if (consentStrategy != Abstractions.InAppConsentStrategy.NotEnabled)
            {
                specificConfig.EnableInAppMessaging(GetInAppConsentStrategy());
            }

            JSONObject sdkInfo = new JSONObject();
            sdkInfo.Put("id", Abstractions.Consts.SDK_TYPE);
            sdkInfo.Put("version", Abstractions.Consts.SDK_VERSION);

            specificConfig.SetSdkInfo(sdkInfo);

            JSONObject runtimeInfo = new JSONObject();
            runtimeInfo.Put("id", Abstractions.Consts.RUNTIME_TYPE);
            runtimeInfo.Put("version", Build.VERSION.Release);

            specificConfig.SetRuntimeInfo(runtimeInfo);

            return specificConfig.Build();
        }

        private Android.KumulosConfig.InAppConsentStrategy GetInAppConsentStrategy()
        {
            if (consentStrategy == Abstractions.InAppConsentStrategy.AutoEnroll)
            {
                return Android.KumulosConfig.InAppConsentStrategy.AutoEnroll;
            }

            if (consentStrategy == Abstractions.InAppConsentStrategy.ExplicitByUser)
            {
                return Android.KumulosConfig.InAppConsentStrategy.ExplicitByUser;
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
