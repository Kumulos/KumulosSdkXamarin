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
    public class KumulosImplementation : IKumulos
    {
        private iOS.Kumulos thisRef;

        public Build Build { get; private set; }

        public PushChannels PushChannels { get; private set; }

        public void Initialize(IKSConfig config)
        {
            var iosKSConfig = (KSConfigImplementation)config;

            thisRef = iOS.Kumulos.InitializeWithConfig(iosKSConfig.Build());

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

        public void RegisterDeviceToken(object NSDataDeviceToken)
        {
            iOS.Kumulos_Push.PushRegisterWithDeviceToken(thisRef, (NSData)NSDataDeviceToken);
        }

        public void TrackNotificationOpen(object NSDictionaryUserInfo)
        {
            iOS.Kumulos_Push.PushTrackOpenFromNotification(thisRef, (NSDictionary)NSDictionaryUserInfo);
        }

        public void TrackEvent(string eventType, Dictionary<string, object> properties)
        {
            var nsDict = ConvertDictionaryToNSDictionary(properties);

            iOS.Kumulos_Analytics.TrackEvent(thisRef, eventType, nsDict);
        }

        public void TrackEventImmediately(string eventType, Dictionary<string, object> properties)
        {

            iOS.Kumulos_Analytics.TrackEventImmediately(thisRef, eventType, ConvertDictionaryToNSDictionary(properties));
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

        private void AttemptToLogException(Exception e , bool uncaught)
        {
            try
            {
                var dict = GetDictionaryForException(e, uncaught);
                WriteCrashToDisk(dict);
            }
            catch (Exception ex)
            {
                // Dont cause
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
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filename = Path.Combine(documents, "CrashLog.json");
            File.WriteAllText(filename, JsonConvert.SerializeObject(crash, Formatting.None));
        }

        private void LogPreviousCrash()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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

        public bool IsGooglePlayServicesAvailable()
        {
            throw new NotImplementedException("This method should not be called on iOS");
        }

        public void TrackNotificationOpen(string notificationId)
        {
            throw new NotImplementedException("This method should not be called on iOS");
        }
    }
}
