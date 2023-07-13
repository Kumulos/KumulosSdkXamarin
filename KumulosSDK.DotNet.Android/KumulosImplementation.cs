using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using KumulosSDK.DotNet.Abstractions;
using Java.Lang;
using Java.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.Json;

using AndroidNative = Com.Kumulos.Android;


namespace KumulosSDK.DotNet.Android
{
    public class InAppDeepLinkHandlerAbstraction : Java.Lang.Object, AndroidNative.IInAppDeepLinkHandlerInterface
    {
        private IInAppDeepLinkHandler handler;

        public InAppDeepLinkHandlerAbstraction(IInAppDeepLinkHandler handler)
        {
            this.handler = handler;
        }

        
        void AndroidNative.IInAppDeepLinkHandlerInterface.Handle(Context? context, AndroidNative.IInAppDeepLinkHandlerInterface.InAppButtonPress? buttonPress)
        {
            var messageData = buttonPress.MessageData != null
                ? JObject.Parse(buttonPress.MessageData.ToString())
                : null;
           
            var deepLinkData = JObject.Parse(buttonPress.DeepLinkData.ToString());

            handler.Handle(new InAppButtonPress(buttonPress.MessageId, messageData, deepLinkData));
        }
    }

    public class InboxUpdatedHandlerAbstraction : Java.Lang.Object, IRunnable, AndroidNative.KumulosInApp.IInAppInboxUpdatedHandler
    {
        private IInboxUpdatedHandler handler;

        public InboxUpdatedHandlerAbstraction(IInboxUpdatedHandler handler)
        {
            this.handler = handler;
        }

        public void Run()
        {
            handler.Handle();
        }
    }

    public class InboxSummaryHandlerAbstraction : Java.Lang.Object, IRunnable, AndroidNative.KumulosInApp.IInAppInboxSummaryHandler
    {
        private TaskCompletionSource<InAppInboxSummary> promise;

        public InboxSummaryHandlerAbstraction(TaskCompletionSource<InAppInboxSummary> promise)
        {
            this.promise = promise;
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public void Run(AndroidNative.InAppInboxSummary inAppInboxSummary)
        {
            var abstractSummary = new InAppInboxSummary(inAppInboxSummary.UnreadCount, inAppInboxSummary.TotalCount);
            promise.TrySetResult(abstractSummary);

        }
    }

    public class KumulosImplementation : KumulosBaseImplementation, IKumulos
    {
        public override void Initialize(IKSConfig config)
        {
            var androidConfig = (KSConfigImplementation)config;

            AndroidNative.Kumulos.Initialize((Application)Application.Context.ApplicationContext, androidConfig.GetConfig());

            if (androidConfig.InAppDeepLinkHandler != null)
            {
                AndroidNative.KumulosInApp.SetDeepLinkHandler(new InAppDeepLinkHandlerAbstraction(androidConfig.InAppDeepLinkHandler));
            }

            base.Initialize(config);
        }

        public override string InstallId
        {
            get
            {
                return AndroidNative.Installation.Id(Application.Context.ApplicationContext);
            }
        }

        public string UserIdentifier
        {
            get
            {
                return AndroidNative.Kumulos.GetCurrentUserIdentifier(Application.Context.ApplicationContext);
            }
        }

        public void UpdateInAppConsentForUser(bool consentGiven)
        {
            AndroidNative.KumulosInApp.UpdateConsentForUser(consentGiven);
        }

        public InAppInboxItem[] InboxItems
        {
            get
            {
                var androidInboxItems = AndroidNative.KumulosInApp.GetInboxItems(Application.Context.ApplicationContext);
                var inboxItems = new InAppInboxItem[androidInboxItems.Count];

                for (var i = 0; i < androidInboxItems.Count; i++)
                {
                    var androidInboxItem = androidInboxItems[i];
                    var imageUrl = androidInboxItem.GetImageUrl(300);

                    inboxItems[i] = new InAppInboxItem(
                        (int)androidInboxItem.Id,
                        androidInboxItem.IsRead,
                        androidInboxItem.Title,
                        androidInboxItem.Subtitle,
                        FromJavaDate(androidInboxItem.SentAt),
                        FromJavaDate(androidInboxItem.AvailableFrom),
                        FromJavaDate(androidInboxItem.AvailableTo),
                        FromJavaDate(androidInboxItem.DismissedAt),
                        imageUrl != null ? imageUrl.ToString() : null,
                        androidInboxItem.Data != null ? JObject.Parse(androidInboxItem.Data.ToString()) : new JObject()
                    );
                }

                return inboxItems;
            }
        }

        private DateTime? FromJavaDate(Date javaDate)
        {
            if (javaDate == null)
            {
                return null;
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(javaDate.Time);
        }

        public InAppMessagePresentationResult PresentInboxMessage(InAppInboxItem item)
        {
            var nativeItem = FindInboxItemForDTO(item);
            var r = AndroidNative.KumulosInApp.PresentInboxMessage(Application.Context.ApplicationContext, nativeItem);

            return MapPresentationResult(r);
        }

        public bool MarkInboxItemAsRead(InAppInboxItem item)
        {
            var nativeItem = FindInboxItemForDTO(item);
            return AndroidNative.KumulosInApp.MarkAsRead(Application.Context.ApplicationContext, nativeItem);
        }

        public bool MarkAllInboxItemsAsRead()
        {
            return AndroidNative.KumulosInApp.MarkAllInboxItemsAsRead(Application.Context.ApplicationContext);
        }

        public Task<InAppInboxSummary> GetInboxSummary()
        {
            var promise = new TaskCompletionSource<InAppInboxSummary>();

            var handlerAbstraction = new InboxSummaryHandlerAbstraction(promise);

            AndroidNative.KumulosInApp.GetInboxSummaryAsync(Application.Context.ApplicationContext, handlerAbstraction);

            return promise.Task;
        }

        public bool DeleteMessageFromInbox(InAppInboxItem item)
        {
            var nativeItem = FindInboxItemForDTO(item);
            return AndroidNative.KumulosInApp.DeleteMessageFromInbox(Application.Context.ApplicationContext, nativeItem);
        }

        private AndroidNative.InAppInboxItem FindInboxItemForDTO(InAppInboxItem item)
        {
            var androidInboxItems = AndroidNative.KumulosInApp.GetInboxItems(Application.Context.ApplicationContext);
            for (var i = 0; i < androidInboxItems.Count; i++)
            {
                if (androidInboxItems[i].Id == item.Id)
                {
                    return androidInboxItems[i];
                }
            }
            throw new System.Exception("Failed to find inbox item for DTO");
        }

        private InAppMessagePresentationResult MapPresentationResult(AndroidNative.KumulosInApp.InboxMessagePresentationResult r)
        {
            if (r == AndroidNative.KumulosInApp.InboxMessagePresentationResult.Presented)
            {
                return InAppMessagePresentationResult.Presented;
            }
            if (r == AndroidNative.KumulosInApp.InboxMessagePresentationResult.FailedExpired)
            {
                return InAppMessagePresentationResult.Expired;
            }

            if (r == AndroidNative.KumulosInApp.InboxMessagePresentationResult.Failed)
            {
                return InAppMessagePresentationResult.Failed;
            }

            throw new System.Exception("Failed to map InAppMessagePresentationResult");
        }

        public void RegisterForRemoteNotifications()
        {
            AndroidNative.Kumulos.PushRequestDeviceToken(Application.Context.ApplicationContext);
        }

        public void UnregisterDeviceToken()
        {
            AndroidNative.Kumulos.PushUnregister(Application.Context.ApplicationContext);
        }

        public override void TrackEvent(string eventType, Dictionary<string, object> properties)
        {
            JSONObject props = new JSONObject(properties);
            AndroidNative.Kumulos.TrackEvent(Application.Context.ApplicationContext, eventType, props);
        }

        public void TrackEventImmediately(string eventType, Dictionary<string, object> properties)
        {
            JSONObject props = new JSONObject(properties);
            AndroidNative.Kumulos.TrackEventImmediately(Application.Context.ApplicationContext, eventType, props);
        }

        public void AssociateUserWithInstall(string userIdentifier)
        {
            AndroidNative.Kumulos.AssociateUserWithInstall(Application.Context.ApplicationContext, userIdentifier);
        }

        public void AssociateUserWithInstall(string userIdentifier, Dictionary<string, object> attributes)
        {
            JSONObject attr = new JSONObject(attributes);

            AndroidNative.Kumulos.AssociateUserWithInstall(Application.Context.ApplicationContext, userIdentifier, attr);
        }

        public void ClearUserAssociation()
        {
            AndroidNative.Kumulos.ClearUserAssociation(Application.Context.ApplicationContext);
        }

        public void SendLocationUpdate(double lat, double lng)
        {
            Location location = new Location("provider");
            location.Latitude = lat;
            location.Longitude = lng;

            AndroidNative.Kumulos.SendLocationUpdate(Application.Context.ApplicationContext, location);
        }

        public void TrackEddystoneBeaconProximity(string namespaceHex, string instanceHex, double distanceMetres)
        {
            Java.Lang.Double dblDistance = new Java.Lang.Double(distanceMetres);
            AndroidNative.Kumulos.TrackEddystoneBeaconProximity(Application.Context.ApplicationContext, namespaceHex, instanceHex, dblDistance);
        }

        public void TrackiBeaconProximity(object CLBeaconObject)
        {
            throw new NotImplementedException("This method should not be called on Android");
        }

        public override void TrackCrashEvent(JObject report)
        {
            JSONObject javaJson = new JSONObject(JsonConvert.SerializeObject(report, Formatting.None));

            AndroidNative.Kumulos.TrackEvent(Application.Context.ApplicationContext, Consts.CRASH_REPORT_EVENT_TYPE, javaJson);
        }

        public void SetPushActionHandler(AndroidNative.IPushActionHandlerInterface pushActionHandler)
        {
            AndroidNative.Kumulos.SetPushActionHandler(pushActionHandler);
        }

        public void SetInboxUpdatedHandler(IInboxUpdatedHandler inboxUpdatedHandler)
        {
            AndroidNative.KumulosInApp.SetOnInboxUpdated(new InboxUpdatedHandlerAbstraction(inboxUpdatedHandler));
        }

        public void ClearInboxUpdatedHandler()
        {
            AndroidNative.KumulosInApp.SetOnInboxUpdated(null);
        }

        public void SeeIntent(Intent intent)
        {
            AndroidNative.Kumulos.SeeIntent(Application.Context.ApplicationContext, intent);
        }

        public void SeeInputFocus(bool hasFocus)
        {
            AndroidNative.Kumulos.SeeInputFocus(Application.Context.ApplicationContext, hasFocus);
        }
    }
}
