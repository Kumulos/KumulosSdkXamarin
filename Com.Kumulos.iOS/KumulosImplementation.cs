using System;
using Com.Kumulos.Abstractions;
using Foundation;
using UIKit;
using UserNotifications;
using System.Net.Http;
using System.Net.Http.Headers;
using CoreLocation;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Com.Kumulos
{
    public class KumulosImplementation : KumulosBaseImplementation, IKumulos
    {
        private iOS.Kumulos thisRef;

        public override void Initialize(IKSConfig config)
        {
            var iosKSConfig = (KSConfigImplementation)config;

            thisRef = iOS.Kumulos.InitializeWithConfig(iosKSConfig.Build());

            base.Initialize(config);
        }

        public override string InstallId
        {
            get
            {
                return iOS.Kumulos.InstallId;
            }
        }

        public string UserIdentifier
        {
            get
            {
                return iOS.Kumulos_Analytics.CurrentUserIdentifier;
            }
        }

        public void UpdateInAppConsentForUser(bool consentGiven)
        {
            iOS.KumulosInApp.UpdateConsentForUser(consentGiven);
        }

        public InAppInboxItem[] InboxItems
        {
            get
            {
                var iosInboxItems = iOS.KumulosInApp.InboxItems;
                var inboxItems = new InAppInboxItem[iosInboxItems.Length];

                for (var i = 0; i < iosInboxItems.Length; i++)
                {
                    var iosInboxItem = iosInboxItems[i];

                    inboxItems[i] = new InAppInboxItem(
                        (int)iosInboxItem.Id,
                        iosInboxItem.Title,
                        iosInboxItem.Subtitle,
                        GetDateTimeFromNSDate(iosInboxItem.AvailableFrom),
                        GetDateTimeFromNSDate(iosInboxItem.AvailableTo),
                        GetDateTimeFromNSDate(iosInboxItem.DismissedAt)
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
            var r = iOS.KumulosInApp.PresentInboxMessage(nativeItem);

            return MapPresentationResult(r);
        }

        private iOS.KSInAppInboxItem FindInboxItemForDTO(InAppInboxItem item)
        {
            var iosInboxItems = iOS.KumulosInApp.InboxItems;
            for (var i = 0; i < iosInboxItems.Length; i++)
            {
                if ((int)iosInboxItems[i].Id == item.Id)
                {
                    return iosInboxItems[i];
                }
            }
            throw new Exception("Failed to find inbox item for DTO");
        }

        private InAppMessagePresentationResult MapPresentationResult(iOS.KSInAppMessagePresentationResult r)
        {
            if (r == iOS.KSInAppMessagePresentationResult.Presented)
            {
                return InAppMessagePresentationResult.Presented;
            }
            if (r == iOS.KSInAppMessagePresentationResult.Expired)
            {
                return InAppMessagePresentationResult.Expired;
            }

            if (r == iOS.KSInAppMessagePresentationResult.Failed)
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
            iOS.Kumulos_Push.PushUnregister(thisRef);
        }

        public override void TrackEvent(string eventType, Dictionary<string, object> properties)
        {
            var nsDict = ConvertDictionaryToNSDictionary(properties);

            iOS.Kumulos_Analytics.TrackEvent(thisRef, eventType, nsDict);
        }

        public void TrackEventImmediately(string eventType, Dictionary<string, object> properties)
        {

            iOS.Kumulos_Analytics.TrackEventImmediately(thisRef, eventType, ConvertDictionaryToNSDictionary(properties));
        }
               
        public void SendLocationUpdate(double lat, double lng)
        {
            CLLocation cl = new CLLocation(lat, lng);

            iOS.Kumulos_Location.SendLocationUpdate(thisRef, cl);
        }

        public void AssociateUserWithInstall(string userIdentifier)
        {
            iOS.Kumulos_Analytics.AssociateUserWithInstall(thisRef, userIdentifier);
        }

        public void AssociateUserWithInstall(string userIdentifier, Dictionary<string, object> attributes)
        {
            var nsDict = NSDictionary.FromObjectsAndKeys(attributes.Values.ToArray(), attributes.Keys.ToArray());

            iOS.Kumulos_Analytics.AssociateUserWithInstall(thisRef, userIdentifier, nsDict);
        }

        public void ClearUserAssociation()
        {
            iOS.Kumulos_Analytics.ClearUserAssociation(thisRef);
        }

        public void TrackiBeaconProximity(object CLBeaconObject)
        {
            iOS.Kumulos_Location.SendiBeaconProximity(thisRef, (CLBeacon)CLBeaconObject);
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
    }
}
