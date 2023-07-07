using CoreLocation;
using KumulosSDK.DotNet.Abstractions;
using Newtonsoft.Json.Linq;
using UserNotifications;

using iOSNative = Com.Kumulos.iOS;

namespace KumulosSDK.DotNet.iOS
{
    public class KumulosImplementation : Abstractions.KumulosBaseImplementation, Abstractions.IKumulos
    {
        private iOSNative.Kumulos thisRef;

        public override void Initialize(IKSConfig config)
        {
            var iosKSConfig = (KSConfigImplementation)config;

            thisRef = iOSNative.Kumulos.InitializeWithConfig(iosKSConfig.Build());

            base.Initialize(config);
        }

        public override string InstallId
        {
            get
            {
                return iOSNative.Kumulos.InstallId;
            }
        }

        public string UserIdentifier
        {
            get
            {
                return iOSNative.Kumulos_Analytics.CurrentUserIdentifier;
            }
        }

        public void UpdateInAppConsentForUser(bool consentGiven)
        {
            iOSNative.KumulosInApp.UpdateConsentForUser(consentGiven);
        }

        public InAppInboxItem[] InboxItems
        {
            get
            {
                var iosInboxItems = iOSNative.KumulosInApp.InboxItems;
                var inboxItems = new InAppInboxItem[iosInboxItems.Length];

                for (var i = 0; i < iosInboxItems.Length; i++)
                {
                    var iosInboxItem = iosInboxItems[i];
                    var imageUrl = iosInboxItem.GetImageUrl(300);
                    var dataPayload = new JObject();

                    if (iosInboxItem.Data != null)
                    {
                        NSError error;
                        var dataJson = NSJsonSerialization.Serialize(iosInboxItem.Data, NSJsonWritingOptions.PrettyPrinted, out error);

                        dataPayload = JObject.Parse(dataJson.ToString());
                    }

                    inboxItems[i] = new InAppInboxItem(
                        (int)iosInboxItem.Id,
                        iosInboxItem.IsRead,
                        iosInboxItem.Title,
                        iosInboxItem.Subtitle,
                        GetDateTimeFromNSDate(iosInboxItem.SentAt),
                        GetDateTimeFromNSDate(iosInboxItem.AvailableFrom),
                        GetDateTimeFromNSDate(iosInboxItem.AvailableTo),
                        GetDateTimeFromNSDate(iosInboxItem.DismissedAt),
                        imageUrl != null ? imageUrl.ToString() : null,
                        dataPayload
                    );
                }

                return inboxItems;
            }
        }

        private DateTime? GetDateTimeFromNSDate(NSDate d)
        {
            if (d == null)
            {
                return null;
            }

            return (DateTime)d;
        }

        public InAppMessagePresentationResult PresentInboxMessage(InAppInboxItem item)
        {
            var nativeItem = FindInboxItemForDTO(item);
            var r = iOSNative.KumulosInApp.PresentInboxMessage(nativeItem);

            return MapPresentationResult(r);
        }

        public bool DeleteMessageFromInbox(InAppInboxItem item)
        {
            var nativeItem = FindInboxItemForDTO(item);
            return iOSNative.KumulosInApp.DeleteMessageFromInbox(nativeItem);
        }

        public Task<InAppInboxSummary> GetInboxSummary()
        {
            var promise = new TaskCompletionSource<InAppInboxSummary>();

            iOSNative.KumulosInApp.GetInboxSummaryAsync((iOSNative.InAppInboxSummary nativeSummary) =>
            {
                var abstractSummary = new InAppInboxSummary(nativeSummary.UnreadCount, nativeSummary.TotalCount);

                promise.TrySetResult(abstractSummary);
            });

            return promise.Task;
        }

        public bool MarkInboxItemAsRead(InAppInboxItem item)
        {
            var nativeItem = FindInboxItemForDTO(item);
            return iOSNative.KumulosInApp.MarkAsRead(nativeItem);
        }

        public bool MarkAllInboxItemsAsRead()
        {
            return iOSNative.KumulosInApp.MarkAllInboxItemsAsRead;
        }

        private iOSNative.KSInAppInboxItem FindInboxItemForDTO(InAppInboxItem item)
        {
            var iosInboxItems = iOSNative.KumulosInApp.InboxItems;
            for (var i = 0; i < iosInboxItems.Length; i++)
            {
                if ((int)iosInboxItems[i].Id == item.Id)
                {
                    return iosInboxItems[i];
                }
            }
            throw new Exception("Failed to find inbox item for DTO");
        }

        private InAppMessagePresentationResult MapPresentationResult(iOSNative.KSInAppMessagePresentationResult r)
        {
            if (r == iOSNative.KSInAppMessagePresentationResult.Presented)
            {
                return InAppMessagePresentationResult.Presented;
            }
            if (r == iOSNative.KSInAppMessagePresentationResult.Expired)
            {
                return InAppMessagePresentationResult.Expired;
            }

            if (r == iOSNative.KSInAppMessagePresentationResult.Failed)
            {
                return InAppMessagePresentationResult.Failed;
            }

            throw new Exception("Failed to map InAppMessagePresentationResult");
        }

        public void RegisterForRemoteNotifications()
        {
            var center = UNUserNotificationCenter.Current;
            center.RequestAuthorization(
                UNAuthorizationOptions.Badge |
                UNAuthorizationOptions.Alert |
                UNAuthorizationOptions.Sound,
                (bool arg1, Foundation.NSError arg2) =>
                {

                });

            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        public void UnregisterDeviceToken()
        {
            iOSNative.Kumulos_Push.PushUnregister(thisRef);
        }

        public bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            return iOSNative.Kumulos_DeepLinking.Application(thisRef, application, userActivity, (_) => { });
        }

        public void Scene(UIScene scene, NSUserActivity userActivity)
        {
            iOSNative.Kumulos_DeepLinking.Scene(thisRef, scene, userActivity);
        }

        public override void TrackEvent(string eventType, Dictionary<string, object> properties)
        {
            var nsDict = ConvertDictionaryToNSDictionary(properties);

            iOSNative.Kumulos_Analytics.TrackEvent(thisRef, eventType, nsDict);
        }

        public void TrackEventImmediately(string eventType, Dictionary<string, object> properties)
        {

            iOSNative.Kumulos_Analytics.TrackEventImmediately(thisRef, eventType, ConvertDictionaryToNSDictionary(properties));
        }

        public void SendLocationUpdate(double lat, double lng)
        {
            CLLocation cl = new CLLocation(lat, lng);

            iOSNative.Kumulos_Location.SendLocationUpdate(thisRef, cl);
        }

        public void AssociateUserWithInstall(string userIdentifier)
        {
            iOSNative.Kumulos_Analytics.AssociateUserWithInstall(thisRef, userIdentifier);
        }

        public void AssociateUserWithInstall(string userIdentifier, Dictionary<string, object> attributes)
        {
            var nsDict = NSDictionary.FromObjectsAndKeys(attributes.Values.ToArray(), attributes.Keys.ToArray());

            iOSNative.Kumulos_Analytics.AssociateUserWithInstall(thisRef, userIdentifier, nsDict);
        }

        public void ClearUserAssociation()
        {
            iOSNative.Kumulos_Analytics.ClearUserAssociation(thisRef);
        }

        public void TrackiBeaconProximity(object CLBeaconObject)
        {
            iOSNative.Kumulos_Location.SendiBeaconProximity(thisRef, (CLBeacon)CLBeaconObject);
        }

        private NSDictionary ConvertDictionaryToNSDictionary(Dictionary<string, object> dict)
        {
            var complexPairs = new List<KeyValuePair<NSObject, NSObject>>();

            var basicKeys = new List<object>();
            var basicValues = new List<object>();


            foreach (KeyValuePair<string, object> entry in dict)
            {
                NSString key = new NSString(entry.Key);

                var value = entry.Value;

                if (value.GetType() == typeof(Dictionary<string, object>))
                {
                    var subDictionary = ConvertDictionaryToNSDictionary((Dictionary<string, object>)value);
                    complexPairs.Add(new KeyValuePair<NSObject, NSObject>(key, subDictionary));
                    continue;
                }

                basicKeys.Add(key);
                basicValues.Add(value);
            }

            var nsDict = NSMutableDictionary.FromObjectsAndKeys(basicValues.ToArray(), basicKeys.ToArray());
            foreach (KeyValuePair<NSObject, NSObject> complex in complexPairs)
            {
                nsDict.Add(complex.Key, complex.Value);
            }

            return nsDict;
        }

        public void TrackEddystoneBeaconProximity(string namespaceHex, string instanceHex, double distanceMetres)
        {
            throw new NotImplementedException("This method should not be called on iOS");
        }

        public override void TrackCrashEvent(JObject report)
        {
            var dict = new Dictionary<string, object>
            {
                { "format", (string)report["format"] },
                { "uncaught", (bool)report["uncaught"] }
            };

            var nestedReport = (JContainer)report["report"];

            var reportDict = new Dictionary<string, object>
            {
                { "stackTrace", (string)nestedReport["stackTrace"] },
                { "message", (string)nestedReport["message"] },
                { "type", (string)nestedReport["type"] },
                { "source", (string)nestedReport["source"] },
                { "lineNumber", (int)nestedReport["lineNumber"] }
            };

            dict.Add("report", reportDict);

            TrackEvent(Consts.CRASH_REPORT_EVENT_TYPE, dict);
        }

        public void SetInboxUpdatedHandler(IInboxUpdatedHandler inboxUpdatedHandler)
        {
            iOSNative.KumulosInApp.SetOnInboxUpdated(() =>
            {
                inboxUpdatedHandler.Handle();
            });
        }

        public void ClearInboxUpdatedHandler()
        {
            iOSNative.KumulosInApp.SetOnInboxUpdated(null);
        }


    }
}
