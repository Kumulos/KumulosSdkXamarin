using System;
using Com.Kumulos.Abstractions;
using Android.App;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Collections.Generic;
using Org.Json;
using Android.Locations;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Android.Gms.Common;
using Java.Util;

namespace Com.Kumulos
{
    public class KumulosImplementation : IKumulos
    {
        public Build Build { get; private set; }

        public PushChannels PushChannels { get; private set; }

        public void Initialize(IKSConfig config)
        {
            var androidConfig = (KSConfigImplementation)config;

            Android.Kumulos.Initialize((Application)Application.Context.ApplicationContext, androidConfig.GetConfig());

            var httpClient = new HttpClient();

            httpClient.MaxResponseContentBufferSize = 256000;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", config.GetApiKey(), config.GetSecretKey())
            )));


            Build = new Build(InstallId, httpClient, config.GetApiKey());
            PushChannels = new PushChannels(InstallId, httpClient);

            try
            {
                LogPreviousCrash();
            }
            catch (Exception e)
            {
                //- Don't cause further exceptions trying to log exceptions.
            }
        }

        public string InstallId
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

                Java.Util.Date d = new Java.Util.Date();


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

        public void TrackEvent(string eventType, Dictionary<string, object> properties)
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

        public void LogException(Exception e)
        {
            AttemptToLogException(e, false);
        }

        public void LogUncaughtException(Exception e)
        {
            AttemptToLogException(e, true);
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

        private void AttemptToLogException(Exception e, bool uncaught)
        {
            try
            {
                var dict = GetDictionaryForException(e, uncaught);
                WriteCrashToDisk(dict);
            }
            catch (Exception ex)
            {
                //- Don't cause further exceptions trying to log exceptions.
            }
        }

        private Dictionary<string, object> GetDictionaryForException(Exception e, bool uncaught)
        {
            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();

            var dict = Crash.GetDictionaryForExceptionTracking(e, uncaught);

            var report = (Dictionary<string, object>)dict["report"];
            report.Add("lineNumber", line);

            return dict;
        }

        private void WriteCrashToDisk(Dictionary<string, object> crash)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filename = Path.Combine(documents, "CrashLog.json");
            File.WriteAllText(filename, JsonConvert.SerializeObject(crash, Formatting.None));
        }

        private void LogPreviousCrash()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filename = Path.Combine(documents, "CrashLog.json");

            if (!File.Exists(filename))
            {
                return;
            }

            var text = File.ReadAllText(filename);
            var jsonObj = (JContainer)JsonConvert.DeserializeObject(text);

            var dict = new Dictionary<string, object>
                {
                    { "format", (string)jsonObj["format"] },
                    { "uncaught", (bool)jsonObj["uncaught"] }
                };

            var reportObj = (JContainer)jsonObj["report"];

            var report = new Dictionary<string, object>
            {
                { "stackTrace", (string)reportObj["stackTrace"] },
                { "message", (string)reportObj["message"] },
                { "type", (string)reportObj["type"] },
                { "source", (string)reportObj["source"] },
                { "lineNumber", (int)reportObj["lineNumber"] }
            };

            dict.Add("report", report);

            TrackEvent(Consts.CRASH_REPORT_EVENT_TYPE, dict);

            File.Delete(filename);
        }

        public void TrackiBeaconProximity(object CLBeaconObject)
        {
            throw new NotImplementedException("This method should not be called on Android");
        }
    }
}
