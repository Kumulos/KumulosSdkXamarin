using System;
using Android.OS;
using Org.Json;
using KumulosSDK.DotNet.Abstractions;
using AndroidOS = Android.OS;

namespace KumulosSDK.DotNet.Android
{
    public class KSConfigImplementation : IKSConfig
    {
        private string apiKey, secretKey;
        private int timeoutSeconds = -1;
        private InAppConsentStrategy consentStrategy = InAppConsentStrategy.NotEnabled;
        private int? notificationSmallIconId;
        private string deepLinkCname;

        public IInAppDeepLinkHandler InAppDeepLinkHandler { get; private set; }
        public IDeepLinkHandler DeepLinkHandler { get; private set; }

        public IKSConfig AddKeys(string apiKey, string secretKey)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;

            return this;
        }

        public IKSConfig EnableCrashReporting()
        {
            throw new NotImplementedException("Native crash reporting is not available on Android - please refer to the integration guide.");
        }

        public IKSConfig SetSessionIdleTimeout(int timeoutSeconds)
        {
            this.timeoutSeconds = timeoutSeconds;
            return this;
        }

        public IKSConfig EnableInAppMessaging(InAppConsentStrategy consentStrategy)
        {
            this.consentStrategy = consentStrategy;
            return this;
        }

        public IKSConfig SetInAppDeepLinkHandler(IInAppDeepLinkHandler inAppDeepLinkHandler)
        {
            InAppDeepLinkHandler = inAppDeepLinkHandler;
            return this;
        }

        public IKSConfig SetPushSmallIconId(int id)
        {
            notificationSmallIconId = id;
            return this;
        }

        public IKSConfig EnableDeepLinking(IDeepLinkHandler deepLinkHandler)
        {
            DeepLinkHandler = deepLinkHandler;
            return this;
        }

        public IKSConfig EnableDeepLinking(string cname, IDeepLinkHandler deepLinkHandler)
        {
            deepLinkCname = cname;
            DeepLinkHandler = deepLinkHandler;
            return this;
        }

        public Com.Kumulos.Android.KumulosConfig GetConfig()
        {
            var specificConfig = new Com.Kumulos.Android.KumulosConfig.Builder(apiKey, secretKey);

            if (timeoutSeconds > -1)
            {
                specificConfig.SetSessionIdleTimeoutSeconds(timeoutSeconds);
            }

            if (consentStrategy != InAppConsentStrategy.NotEnabled)
            {
                specificConfig.EnableInAppMessaging(GetInAppConsentStrategy());
            }

            if (DeepLinkHandler != null)
            {
                var abstraction = new DeepLinkHandlerAbstraction(DeepLinkHandler);
                if (deepLinkCname != null)
                {
                    specificConfig.EnableDeepLinking(deepLinkCname, abstraction);
                }
                else
                {
                    specificConfig.EnableDeepLinking(abstraction);
                }
            }


            JSONObject sdkInfo = new JSONObject();
            sdkInfo.Put("id", Consts.SDK_TYPE);
            sdkInfo.Put("version", Consts.SDK_VERSION);

            specificConfig.SetSdkInfo(sdkInfo);

            JSONObject runtimeInfo = new JSONObject();
            runtimeInfo.Put("id", Consts.RUNTIME_TYPE);
            runtimeInfo.Put("version", AndroidOS.Build.VERSION.Release);

            specificConfig.SetRuntimeInfo(runtimeInfo);

            if (notificationSmallIconId.HasValue)
            {
                specificConfig.SetPushSmallIconId(notificationSmallIconId.Value);
            }

            return specificConfig.Build();
        }

        private Com.Kumulos.Android.KumulosConfig.InAppConsentStrategy GetInAppConsentStrategy()
        {
            if (consentStrategy == InAppConsentStrategy.AutoEnroll)
            {
                return Com.Kumulos.Android.KumulosConfig.InAppConsentStrategy.AutoEnroll;
            }

            if (consentStrategy == InAppConsentStrategy.ExplicitByUser)
            {
                return Com.Kumulos.Android.KumulosConfig.InAppConsentStrategy.ExplicitByUser;
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
