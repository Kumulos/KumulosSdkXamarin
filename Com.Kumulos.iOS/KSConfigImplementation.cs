using System;
using Com.Kumulos.Abstractions;
using Foundation;
using Newtonsoft.Json.Linq;
using UserNotifications;

namespace Com.Kumulos
{
    public class KSConfigImplementation : IKSConfig
    {
        private string apiKey, secretKey;
        private bool enableCrashReporting;
        private int timeoutSeconds = -1;
        private InAppConsentStrategy consentStrategy = InAppConsentStrategy.NotEnabled;
        private iOS.KSPushOpenedHandlerBlock pushOpenedHandlerBlock;
        private iOS.KSPushReceivedInForegroundHandlerBlock pushReceivedInForegroundHandlerBlock;
        private UNNotificationPresentationOptions notificationPresentationOptions;

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
            enableCrashReporting = true;
            return this;
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

        public IKSConfig SetPushOpenedHandler(iOS.KSPushOpenedHandlerBlock pushOpenedHandlerBlock)
        {
            this.pushOpenedHandlerBlock = pushOpenedHandlerBlock;
            return this;
        }

        public IKSConfig SetForegroundPushPresentationOptions(UNNotificationPresentationOptions notificationPresentationOptions)
        {
            this.notificationPresentationOptions = notificationPresentationOptions;
            return this;
        }

        public IKSConfig SetPushReceivedInForegroundHandler(iOS.KSPushReceivedInForegroundHandlerBlock pushReceivedInForegroundHandlerBlock)
        {
            this.pushReceivedInForegroundHandlerBlock = pushReceivedInForegroundHandlerBlock;
            return this;
        }

        public IKSConfig EnableDeepLinking(IDeepLinkHandler deepLinkHandler)
        {
            DeepLinkHandler = deepLinkHandler;
            return this;
        }

        public iOS.KSConfig Build()
        {
            var specificConfig = iOS.KSConfig.ConfigWithAPIKey(apiKey, secretKey);

            if (enableCrashReporting)
            {
                specificConfig.EnableCrashReporting();
            }

            if (timeoutSeconds > -1)
            {
                specificConfig.SetSessionIdleTimeout((nuint)timeoutSeconds);
            }

            if (consentStrategy != InAppConsentStrategy.NotEnabled)
            {
                specificConfig.EnableInAppMessaging(GetInAppConsentStrategy());
            }

            if (pushOpenedHandlerBlock != null)
            {
                specificConfig.SetPushOpenedHandler(pushOpenedHandlerBlock);
            }

            specificConfig.SetForegroundPushPresentationOptions(notificationPresentationOptions);

            if (pushReceivedInForegroundHandlerBlock != null)
            {
                specificConfig.SetPushReceivedInForegroundHandler(pushReceivedInForegroundHandlerBlock);
            }

            if (InAppDeepLinkHandler != null)
            {
                specificConfig.SetInAppDeepLinkHandler((NSDictionary target) =>
                {
                    NSError e = new NSError();
                    NSData d = NSJsonSerialization.Serialize(target, NSJsonWritingOptions.PrettyPrinted, out e);
                    JObject o = JObject.Parse(d.ToString());

                    var messageId = Convert.ToInt32(o.Property("MessageId"));
                    var messageData = JObject.Parse(o.Property("MessageData").ToString());
                    var deepLinkData = JObject.Parse(o.Property("DeepLinkData").ToString());

                    InAppDeepLinkHandler.Handle(new InAppButtonPress(messageId, messageData, deepLinkData));
                });
            }

            //if (DeepLinkHandler != null)
            //{
            //    specificConfig.EnableDeepLinking((iOS.KSDeepLinkResolution resolution, NSUrl url, iOS.KSDeepLink? deepLink, AsyncCallback callback, object @object)) {
            //        DeepLinkHandler.Handle()
            //    });
            //}

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

        private DeepLinkResolution MapDeeplinkResolution(iOS.KSDeepLinkResolution deepLinkResolution)
        {
            if (deepLinkResolution == iOS.KSDeepLinkResolution.ookupFailed)
            {
                return DeepLinkResolution.LookupFailed;
            }
            else if (deepLinkResolution == iOS.KSDeepLinkResolution.inkNotFound)
            {
                return DeepLinkResolution.LinkNotFound;
            }
            else if (deepLinkResolution == iOS.KSDeepLinkResolution.inkExpired)
            {
                return DeepLinkResolution.LinkExpired;
            }
            else if (deepLinkResolution == iOS.KSDeepLinkResolution.inkLimitExceeded)
            {
                return DeepLinkResolution.LinkLimitExceeded;
            }
            else if (deepLinkResolution == iOS.KSDeepLinkResolution.inkMatched)
            {
                return DeepLinkResolution.LinkMatched;
            }

            throw new Exception("Failed to map DeepLinkResolution");
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
