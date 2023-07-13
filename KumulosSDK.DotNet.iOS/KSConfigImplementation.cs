using System;
using KumulosSDK.DotNet.Abstractions;
using Foundation;
using Newtonsoft.Json.Linq;
using UserNotifications;

using iOSNative = Com.Kumulos.iOS;

namespace KumulosSDK.DotNet.iOS
{
    public class KSConfigImplementation : IKSConfig
    {
        private string apiKey, secretKey;
        private bool enableCrashReporting;
        private int timeoutSeconds = -1;
        private InAppConsentStrategy consentStrategy = InAppConsentStrategy.NotEnabled;
        private iOSNative.KSPushOpenedHandlerBlock pushOpenedHandlerBlock;
        private iOSNative.KSPushReceivedInForegroundHandlerBlock pushReceivedInForegroundHandlerBlock;
        private UNNotificationPresentationOptions notificationPresentationOptions;
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

        public IKSConfig SetPushOpenedHandler(iOSNative.KSPushOpenedHandlerBlock pushOpenedHandlerBlock)
        {
            this.pushOpenedHandlerBlock = pushOpenedHandlerBlock;
            return this;
        }

        public IKSConfig SetForegroundPushPresentationOptions(UNNotificationPresentationOptions notificationPresentationOptions)
        {
            this.notificationPresentationOptions = notificationPresentationOptions;
            return this;
        }

        public IKSConfig SetPushReceivedInForegroundHandler(iOSNative.KSPushReceivedInForegroundHandlerBlock pushReceivedInForegroundHandlerBlock)
        {
            this.pushReceivedInForegroundHandlerBlock = pushReceivedInForegroundHandlerBlock;
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

        public iOSNative.KSConfig Build()
        {
            var specificConfig = iOSNative.KSConfig.ConfigWithAPIKey(apiKey, secretKey);

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
                specificConfig.SetInAppDeepLinkHandler((iOSNative.KSInAppButtonPress buttonPress) =>
                {
                    NSError e = new NSError();

                    NSData deepLinkData = NSJsonSerialization.Serialize(buttonPress.DeepLinkData, NSJsonWritingOptions.PrettyPrinted, out e);
                    JObject deepLinkDataJObject = JObject.Parse(deepLinkData.ToString());

                    NSData messageData = NSJsonSerialization.Serialize(buttonPress.MessageData, NSJsonWritingOptions.PrettyPrinted, out e);
                    JObject messageDataJObject = JObject.Parse(messageData.ToString());

                    InAppDeepLinkHandler.Handle(new InAppButtonPress(buttonPress.MessageId.Int32Value, messageDataJObject, deepLinkDataJObject));
                });
            }

            if (DeepLinkHandler != null)
            {
                if (deepLinkCname != null)
                {
                    specificConfig.EnableDeepLinking(deepLinkCname, (iOSNative.KSDeepLinkResolution deepLinkResolution, NSUrl url, iOSNative.KSDeepLink deeplink) =>
                    {
                        var uri = new Uri(url.ToString());
                        var deeplinkAbstraction = deeplink != null ? MapDeeplinkObject(deeplink) : null;
                        DeepLinkHandler.Handle(MapDeeplinkResolution(deepLinkResolution), uri, deeplinkAbstraction);
                    });
                }
                else
                {
                    specificConfig.EnableDeepLinking((iOSNative.KSDeepLinkResolution deepLinkResolution, NSUrl url, iOSNative.KSDeepLink deeplink) =>
                    {
                        var uri = new Uri(url.ToString());
                        var deeplinkAbstraction = deeplink != null ? MapDeeplinkObject(deeplink) : null;
                        DeepLinkHandler.Handle(MapDeeplinkResolution(deepLinkResolution), uri, deeplinkAbstraction);
                    });
                }
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

        private DeepLinkResolution MapDeeplinkResolution(iOSNative.KSDeepLinkResolution deepLinkResolution)
        {
            if (deepLinkResolution == iOSNative.KSDeepLinkResolution.ookupFailed)
            {
                return DeepLinkResolution.LookupFailed;
            }
            else if (deepLinkResolution == iOSNative.KSDeepLinkResolution.inkNotFound)
            {
                return DeepLinkResolution.LinkNotFound;
            }
            else if (deepLinkResolution == iOSNative.KSDeepLinkResolution.inkExpired)
            {
                return DeepLinkResolution.LinkExpired;
            }
            else if (deepLinkResolution == iOSNative.KSDeepLinkResolution.inkLimitExceeded)
            {
                return DeepLinkResolution.LinkLimitExceeded;
            }
            else if (deepLinkResolution == iOSNative.KSDeepLinkResolution.inkMatched)
            {
                return DeepLinkResolution.LinkMatched;
            }

            throw new Exception("Failed to map DeepLinkResolution");
        }

        private DeepLink MapDeeplinkObject(iOSNative.KSDeepLink deepLink)
        {
            NSError e = new NSError();
            NSData d = NSJsonSerialization.Serialize(deepLink.Data, NSJsonWritingOptions.PrettyPrinted, out e);
            JObject o = JObject.Parse(d.ToString());

            return new DeepLink(new Uri(deepLink.Url.ToString()), MapDeepLinkContent(deepLink.Content), o);
        }

        private DeepLinkContent MapDeepLinkContent(iOSNative.KSDeepLinkContent deepLinkContent)
        {
            if (deepLinkContent == null)
            {
                return null;
            }
            return new DeepLinkContent(deepLinkContent.Title, deepLinkContent.Description);
        }

        private iOSNative.KSInAppConsentStrategy GetInAppConsentStrategy()
        {
            if (consentStrategy == InAppConsentStrategy.AutoEnroll)
            {
                return iOSNative.KSInAppConsentStrategy.AutoEnroll;
            }

            if (consentStrategy == InAppConsentStrategy.ExplicitByUser)
            {
                return iOSNative.KSInAppConsentStrategy.ExplicitByUser;
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
