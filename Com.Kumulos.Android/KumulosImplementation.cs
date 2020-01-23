using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Android.App;
using Android.Content;
using Android.Locations;
using Com.Kumulos.Abstractions;

using Java.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.Json;

namespace Com.Kumulos
{
    public class DeepLinkHandlerAbstraction : Java.Lang.Object, Android.IInAppDeepLinkHandlerInterface
    {
        private IInAppDeepLinkHandler handler;

        public DeepLinkHandlerAbstraction(IInAppDeepLinkHandler handler)
        {
            this.handler = handler;
        }

        void Android.IInAppDeepLinkHandlerInterface.Handle(Context context, JSONObject data)
        {
            handler.Handle(JObject.Parse(data.ToString()));
        }
    }

    public class KumulosImplementation :  KumulosBaseImplementation, IKumulos
    {
        public override void Initialize(IKSConfig config)
        {
            var androidConfig = (KSConfigImplementation)config;

            Android.Kumulos.Initialize((Application)Application.Context.ApplicationContext, androidConfig.GetConfig());

            if (androidConfig.InAppDeepLinkHandler != null)
            {
                Android.KumulosInApp.SetDeepLinkHandler(new DeepLinkHandlerAbstraction(androidConfig.InAppDeepLinkHandler));
            }

            base.Initialize(config);
        }

        public override string InstallId
        {
            get
            {
                return Android.Installation.Id(Application.Context.ApplicationContext);
            }
        }

        public string UserIdentifier
        {
            get
            {
                return Android.Kumulos.GetCurrentUserIdentifier(Application.Context.ApplicationContext);
            }
        }

        public void UpdateInAppConsentForUser(bool consentGiven)
        {
            Android.KumulosInApp.UpdateConsentForUser(consentGiven);
        }

        public InAppInboxItem[] InboxItems
        {
            get
            {
                var androidInboxItems = Android.KumulosInApp.GetInboxItems(Application.Context.ApplicationContext);
                var inboxItems = new InAppInboxItem[androidInboxItems.Count];

                for (var i = 0; i < androidInboxItems.Count; i++)
                {
                    var androidInboxItem = androidInboxItems[i];
                    inboxItems[i] = new InAppInboxItem(
                        (int)androidInboxItem.Id,
                        androidInboxItem.Title,
                        androidInboxItem.Subtitle,
                        FromJavaDate(androidInboxItem.AvailableFrom),
                        FromJavaDate(androidInboxItem.AvailableTo),
                        FromJavaDate(androidInboxItem.DismissedAt)
                    );
                }

                return inboxItems;
            }
        }

        public DateTime? FromJavaDate(Date javaDate)
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
            var r = Android.KumulosInApp.PresentInboxMessage(Application.Context.ApplicationContext, nativeItem);

            return MapPresentationResult(r);
        }

        private Android.InAppInboxItem FindInboxItemForDTO(InAppInboxItem item)
        {
            var androidInboxItems = Android.KumulosInApp.GetInboxItems(Application.Context.ApplicationContext);
            for (var i = 0; i < androidInboxItems.Count; i++)
            {
                if (androidInboxItems[i].Id == item.Id)
                {
                    return androidInboxItems[i];
                }
            }
            throw new Exception("Failed to find inbox item for DTO");
        }

        private InAppMessagePresentationResult MapPresentationResult(Android.KumulosInApp.InboxMessagePresentationResult r)
        {
            if (r == Android.KumulosInApp.InboxMessagePresentationResult.Presented)
            {
                return InAppMessagePresentationResult.Presented;
            }
            if (r == Android.KumulosInApp.InboxMessagePresentationResult.FailedExpired)
            {
                return InAppMessagePresentationResult.Expired;
            }

            if (r == Android.KumulosInApp.InboxMessagePresentationResult.Failed)
            {
                return InAppMessagePresentationResult.Failed;
            }

            throw new Exception("Failed to map InAppMessagePresentationResult");
        }
        
        public void RegisterForRemoteNotifications()
        {
            Android.Kumulos.PushRegister(Application.Context.ApplicationContext);
        }

        public void UnregisterDeviceToken()
        {
            Android.Kumulos.PushUnregister(Application.Context.ApplicationContext);
        }

        public override void TrackEvent(string eventType, Dictionary<string, object> properties)
        {
            JSONObject props = new JSONObject(properties);
            Android.Kumulos.TrackEvent(Application.Context.ApplicationContext, eventType, props);
        }

        public void TrackEventImmediately(string eventType, Dictionary<string, object> properties)
        {
            JSONObject props = new JSONObject(properties);
            Android.Kumulos.TrackEventImmediately(Application.Context.ApplicationContext, eventType, props);
        }

        public void AssociateUserWithInstall(string userIdentifier)
        {
            Android.Kumulos.AssociateUserWithInstall(Application.Context.ApplicationContext, userIdentifier);
        }

        public void AssociateUserWithInstall(string userIdentifier, Dictionary<string, object> attributes)
        {
            JSONObject attr = new JSONObject(attributes);

            Android.Kumulos.AssociateUserWithInstall(Application.Context.ApplicationContext, userIdentifier, attr);
        }

        public void ClearUserAssociation()
        {
            Android.Kumulos.ClearUserAssociation(Application.Context.ApplicationContext);
        }

        public void SendLocationUpdate(double lat, double lng)
        {
            Location location = new Location("provider");
            location.Latitude = lat;
            location.Longitude = lng;

            Android.Kumulos.SendLocationUpdate(Application.Context.ApplicationContext, location);
        }

        public void TrackEddystoneBeaconProximity(string namespaceHex, string instanceHex, double distanceMetres)
        {
            Java.Lang.Double dblDistance = new Java.Lang.Double(distanceMetres);
            Android.Kumulos.TrackEddystoneBeaconProximity(Application.Context.ApplicationContext, namespaceHex, instanceHex, dblDistance);
        }
        
        public void TrackiBeaconProximity(object CLBeaconObject)
        {
            throw new NotImplementedException("This method should not be called on Android");
        }

        public override void TrackCrashEvent(JObject report)
        {
            JSONObject javaJson = new JSONObject(JsonConvert.SerializeObject(report, Formatting.None));

            Android.Kumulos.TrackEvent(Application.Context.ApplicationContext, Consts.CRASH_REPORT_EVENT_TYPE, javaJson);
        }

        public void SetPushActionHandler(Android.IPushActionHandlerInterface pushActionHandler)
        {
            Android.Kumulos.SetPushActionHandler(pushActionHandler);
        }
    }
}
